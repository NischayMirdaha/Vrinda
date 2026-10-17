using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Net.Http.Headers;
using VrindavanProfile.Web.Data;
using VrindavanProfile.Web.Data.DbEntities;
using VrindavanProfile.Web.Services;
using VrindavanProfile.Web.Services.Interfaces;
using VrindavanProfile.Web.Services.Caching;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

var builder = WebApplication.CreateBuilder(args);

// ─── MVC ───────────────────────────────────────────────
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<CacheInvalidationInterceptor>();

// ─── PostgreSQL + EF Core ──────────────────────────────
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(serviceProvider.GetRequiredService<CacheInvalidationInterceptor>());
});

// ─── In-Memory Cache ───────────────────────────────────
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 500;
});

// ─── Response Compression ──────────────────────────────
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults
        .MimeTypes.Concat(new[]
        {
            "application/json",
            "application/javascript",
            "text/css",
            "text/html",
            "text/plain",
            "image/svg+xml"
        });
});

// ─── Data Protection (D2): Persist keys to disk ────────
// Prevents users from being forcibly logged out every time the app is restarted.
// Keys are stored in a 'DataProtectionKeys' folder next to the app on the Windows server.
var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetApplicationName("VrindavanProfile");

// ─── Forward Headers (D3): For proxy / domain routing ──
// Ensures Request.Scheme reflects "https" when behind IIS, ngrok, or a router NAT.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Trust all proxies on the local network (safe for a single Windows PC server)
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddHttpContextAccessor();

// ─── Application Services ──────────────────────────────
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IHouseholdService, HouseholdService>();
builder.Services.AddScoped<IInstitutionService, InstitutionService>();
builder.Services.AddScoped<IWardService, WardService>();
builder.Services.AddScoped<IMapService, MapService>();

// ─── Authentication & Cookies (D4) ─────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath         = "/Auth/Login";
        options.AccessDeniedPath  = "/Home/Forbidden";
        options.ExpireTimeSpan    = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        // Secure cookie in Production (HTTPS domain) — allows HTTP in Development
        options.Cookie.SecurePolicy = builder.Environment.IsProduction()
            ? CookieSecurePolicy.Always
            : CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
        options.Cookie.Name     = "VrindavanAuth";
    });

// ─── Build ─────────────────────────────────────────────
var app = builder.Build();

// ─── Forward Headers Middleware — must be first ────────
app.UseForwardedHeaders();

// ─── Error Handling & HTTPS ────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS: tell browsers to always use HTTPS for this domain (1 year)
    app.UseHsts();
    // Force HTTPS redirect in Production (Kestrel running directly on the Windows PC server)
    app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.UseResponseCompression();

// ─── Static Files with long-lived cache (D5) ───────────
// Files served with asp-append-version="true" get a fingerprint query param (?v=...),
// so browsers safely cache them for 1 year without ever serving stale content.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Long-lived cache for fingerprinted assets (those with a "v=" query param)
        if (ctx.Context.Request.Query.ContainsKey("v"))
        {
            ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                "public,max-age=31536000,immutable";
        }
        else
        {
            // Short-lived cache for non-versioned files (favicon, robots.txt, PDFs, etc.)
            ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=3600";
        }
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ─── Admin User Seeding ────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        // Check if any admin/user exists — skipped on subsequent starts (fast path)
        if (!await db.AppUsers.AnyAsync())
        {
            db.AppUsers.Add(new AppUser
            {
                Username     = "admin",
                Email        = "admin@vrindavanmun.gov.np",
                PasswordHash = PasswordHasher.Hash("Admin@2081"),
                Role         = "admin",
                CreatedAt    = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        // ─── Auto Recalculate / Populate Ages and DOBs ───
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Checking family member ages and DOBs...");

        bool updatedAny = false;

        // 1. Populate missing DobBs for members who only have Age
        var membersWithoutDob = await db.FamilyMembers
            .Where(fm => (fm.DobBs == null || fm.DobBs == "") && fm.Age != null && fm.Age > 0)
            .ToListAsync();

        if (membersWithoutDob.Any())
        {
            logger.LogInformation("Calculating approximate DOB for {Count} family members with manual age...", membersWithoutDob.Count);
            foreach (var fm in membersWithoutDob)
            {
                fm.DobBs = NepaliDateHelper.ApproxDobBsFromAge((int)fm.Age.Value);
                updatedAny = true;
            }
        }

        // 2. Update out-of-sync ages based on DobBs
        var membersWithDob = await db.FamilyMembers
            .Where(fm => fm.DobBs != null && fm.DobBs != "")
            .ToListAsync();

        int outOfSyncCount = 0;
        foreach (var fm in membersWithDob)
        {
            var calculatedAge = NepaliDateHelper.CalculateAgeBs(fm.DobBs);
            if (calculatedAge.HasValue && fm.Age != calculatedAge.Value)
            {
                fm.Age = calculatedAge.Value;
                outOfSyncCount++;
                updatedAny = true;
            }
        }

        if (outOfSyncCount > 0)
        {
            logger.LogInformation("Updating ages for {Count} members whose birthday has passed...", outOfSyncCount);
        }

        if (updatedAny)
        {
            await db.SaveChangesAsync();
            logger.LogInformation("Saved family member age/DOB updates successfully.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding or recalculating startup family member data.");
    }
}

app.Run();
