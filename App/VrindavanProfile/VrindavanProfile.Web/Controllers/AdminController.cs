using Microsoft.AspNetCore.Authorization;
 using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VrindavanProfile.Web.Data;
using VrindavanProfile.Web.Data.DbEntities;
using VrindavanProfile.Web.Models.ViewModels;
using VrindavanProfile.Web.Services;

namespace VrindavanProfile.Web.Controllers;

[Authorize(Roles = "admin,ward_admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }
    
    private int? UserWard =>
        int.TryParse(User.FindFirst("WardNo")?.Value, out var w) && w > 0 ? w : (int?)null;

    /// <summary>True when the logged-in user has the "ward_admin" role.</summary>
    private bool IsWardAdmin => User.IsInRole("ward_admin");
    
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "व्यवस्थापन ड्यासबोर्ड";

        // Simple direct counts — no service methods needed
        var totalHouseholds = await _db.Households.CountAsync();
        
        // For institutions we sum the main ones (we can expand this easily later)
        var totalInstitutions = await _db.EducationalInstitutions.CountAsync()
                                + await _db.HealthInstitutions.CountAsync()
                                + await _db.FinancialInstitutions.CountAsync()
                                + await _db.SecurityBodies.CountAsync()
                                + await _db.HotelResorts.CountAsync();

        ViewBag.TotalHouseholds = totalHouseholds;
        ViewBag.TotalInstitutions = totalInstitutions;
        ViewBag.TotalWards = await _db.Households
            .Where(h => h.WardNo != null)
            .Select(h => h.WardNo)
            .Distinct()
            .CountAsync(); 

        return View();
    }

    [HttpGet]
    public IActionResult CreateSurveyEntry()
    {
        ViewData["Title"] = "नयाँ सर्वेक्षण प्रविष्टि थप्नुहोस्";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSurveyEntry(VrindavanProfile.Web.Data.DbEntities.SurveyEntry model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "नयाँ सर्वेक्षण प्रविष्टि थप्नुहोस्";
            return View(model);
        }

        if (IsWardAdmin && UserWard.HasValue && model.WardNo != UserWard.Value)
        {
            ModelState.AddModelError("WardNo",
                $"तपाईं केवल वडा नं. {UserWard.Value} को डाटा प्रविष्टि गर्न सक्नुहुन्छ।");
            return View(model);
        }

        var nowStr = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        // Build the parent entity using the same EF pattern as CreateInstitution.
        // All timestamp columns are mapped as string? in SurveyEntry, so EF handles them safely.
        var parent = new SurveyEntry
        {
            WardNo          = model.WardNo,
            EnumeratorName  = model.EnumeratorName,
            Username        = model.Username,
            PhoneNumber     = model.PhoneNumber,
            DeviceId        = model.DeviceId,
            SubmittedBy     = model.SubmittedBy,
            Status          = model.Status ?? "submitted",
            ValidationStatus= model.ValidationStatus,
            SubjectArea     = model.SubjectArea,
            Municipality    = model.Municipality ?? "वृन्दावन नगरपालिका",
            ConsentText     = model.ConsentText,
            Notes           = model.Notes,
            Tags            = model.Tags,
            Version         = model.Version,
            OdkId           = model.OdkId,
            OdkIndex        = model.OdkIndex,
            CreatedAt       = nowStr,
            SubmissionTime  = nowStr,
            SurveyStart     = nowStr,
            SurveyEnd       = nowStr,
            SurveyDate      = nowStr,
        };

        _db.SurveyEntries.Add(parent);
        await _db.SaveChangesAsync(); // Generates the parent ID

        // If a recognised SubjectArea was chosen, insert an empty child row so this
        // entry becomes visible in the Institutions list (which queries child tables,
        // not survey_entry.subject_area).
        if (!string.IsNullOrWhiteSpace(parent.SubjectArea)
            && SubjectAreaConfigs.TryGetValue(parent.SubjectArea, out var cfg))
        {
            var child = Activator.CreateInstance(cfg.EntityType)!;
            cfg.EntityType.GetProperty("SurveyEntryId")?.SetValue(child, parent.Id);
            _db.Add(child);
            await _db.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = "नयाँ प्रविष्टि सफलतापूर्वक थपियो।";
        return RedirectToAction(nameof(SurveyEntries));
    }
    
        // ==================== EDIT SURVEY ENTRY ====================

    [HttpGet]
    public async Task<IActionResult> EditSurveyEntry(long id)
    {
        ViewData["Title"] = "सर्वेक्षण प्रविष्टि सम्पादन गर्नुहोस्";

        // Safe projection (never load full entity with timestamptz columns)
        var data = await _db.SurveyEntries
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id,
                s.WardNo,
                s.EnumeratorName,
                s.Username,
                s.PhoneNumber,
                s.DeviceId,
                s.SubmittedBy,
                s.Status,
                s.ValidationStatus,
                s.SubjectArea,
                s.Municipality,
                s.ConsentText,
                s.Notes,
                s.Tags,
                s.Version,
                s.OdkId,
                s.OdkIndex
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return NotFound();
        
        if (IsWardAdmin && UserWard.HasValue && data.WardNo != UserWard.Value)
            return Forbid();

        var model = new VrindavanProfile.Web.Data.DbEntities.SurveyEntry
        {
            Id = data.Id,
            WardNo = data.WardNo,
            EnumeratorName = data.EnumeratorName,
            Username = data.Username,
            PhoneNumber = data.PhoneNumber,
            DeviceId = data.DeviceId,
            SubmittedBy = data.SubmittedBy,
            Status = data.Status,
            ValidationStatus = data.ValidationStatus,
            SubjectArea = data.SubjectArea,
            Municipality = data.Municipality,
            ConsentText = data.ConsentText,
            Notes = data.Notes,
            Tags = data.Tags,
            Version = data.Version,
            OdkId = data.OdkId,
            OdkIndex = data.OdkIndex
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSurveyEntry(VrindavanProfile.Web.Data.DbEntities.SurveyEntry model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "सर्वेक्षण प्रविष्टि सम्पादन गर्नुहोस्";
            return View(model);
        }
        
        if (IsWardAdmin && UserWard.HasValue && model.WardNo != UserWard.Value)
        {
            ModelState.AddModelError("WardNo",
                $"तपाईं केवल वडा नं. {UserWard.Value} को डाटा परिवर्तन गर्न सक्नुहुन्छ।");
            ViewData["Title"] = "सर्वेक्षण प्रविष्टि सम्पादन गर्नुहोस्";
            return View(model);
        }
        
        // Safe update – never materializes the full entity
        await _db.SurveyEntries
            .Where(s => s.Id == model.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.WardNo, model.WardNo)
                .SetProperty(x => x.EnumeratorName, model.EnumeratorName)
                .SetProperty(x => x.Username, model.Username)
                .SetProperty(x => x.PhoneNumber, model.PhoneNumber)
                .SetProperty(x => x.DeviceId, model.DeviceId)
                .SetProperty(x => x.SubmittedBy, model.SubmittedBy)
                .SetProperty(x => x.Status, model.Status)
                .SetProperty(x => x.ValidationStatus, model.ValidationStatus)
                .SetProperty(x => x.SubjectArea, model.SubjectArea)
                .SetProperty(x => x.Municipality, model.Municipality)
                .SetProperty(x => x.ConsentText, model.ConsentText)
                .SetProperty(x => x.Notes, model.Notes)
                .SetProperty(x => x.Tags, model.Tags)
                .SetProperty(x => x.Version, model.Version)
                .SetProperty(x => x.OdkId, model.OdkId)
                .SetProperty(x => x.OdkIndex, model.OdkIndex)
            );

        TempData["SuccessMessage"] = "प्रविष्टि सफलतापूर्वक अपडेट गरियो।";
        return RedirectToAction(nameof(SurveyEntries));
    }
    
    public async Task<IActionResult> SurveyEntries(
        int? wardNo = null,
        string? subjectArea = null,
        string? status = null,
        string? search = null,
        int page = 1,
        int pageSize = 20)
    {
        ViewData["Title"] = "सर्वेक्षण प्रविष्टिहरू";
    
        var query = _db.SurveyEntries.AsNoTracking().Where(s => s.Status != "deleted");
    
        // ─── Filters ───
        if (wardNo.HasValue)
            query = query.Where(s => s.WardNo == wardNo.Value);
    
        if (!string.IsNullOrWhiteSpace(subjectArea))
            query = query.Where(s => s.SubjectArea == subjectArea);
    
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.ValidationStatus == status);
    
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s =>
                (s.EnumeratorName != null && s.EnumeratorName.Contains(search)) ||
                (s.SubmittedBy    != null && s.SubmittedBy.Contains(search))    ||
                (s.Username       != null && s.Username.Contains(search))       ||
                (s.PhoneNumber    != null && s.PhoneNumber.Contains(search))    ||
                (s.Uuid           != null && s.Uuid.Contains(search)));
    
        var totalItems = await query.CountAsync();
        ViewBag.DeletedCount = await _db.SurveyEntries.CountAsync(s => s.Status == "deleted");
    
        var entries = await query
            .OrderByDescending(s => s.Id)
            .Select(s => new {
                s.Id, s.WardNo, s.Uuid, s.EnumeratorName, s.Username,
                s.PhoneNumber, s.DeviceId, s.SubmittedBy, s.Status,
                s.ValidationStatus, s.SubjectArea, s.Municipality,
                s.OdkId, s.OdkIndex, s.Version
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    
        ViewBag.CurrentPage  = page;
        ViewBag.TotalPages   = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.WardNo       = wardNo;
        ViewBag.SubjectArea  = subjectArea;
        ViewBag.Status       = status;
        ViewBag.Search       = search;
        ViewBag.TotalItems   = totalItems;
    
        return View(entries);
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]  
    public async Task<IActionResult> DeleteSurveyEntry(long id)
    {
        await _db.SurveyEntries
            .Where(s => s.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "deleted"));

        TempData["SuccessMessage"] = "प्रविष्टि ट्रेसमा सारियो।";
        return RedirectToAction(nameof(SurveyEntries));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreSurveyEntry(long id)
    {
        await _db.SurveyEntries
            .Where(s => s.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "submitted"));

        TempData["SuccessMessage"] = "प्रविष्टि पुनर्स्थापित गरियो।";
        return RedirectToAction(nameof(SurveyEntries));
    }

    public async Task<IActionResult> SurveyEntriesTrash(int? wardNo = null, int page = 1, int pageSize = 20)
    {
        ViewData["Title"] = "ट्रेस (मेटाइएका प्रविष्टिहरू)";

        var query = _db.SurveyEntries.AsNoTracking().Where(s => s.Status == "deleted");

        if (wardNo.HasValue)
            query = query.Where(s => s.WardNo == wardNo.Value);

        var totalItems = await query.CountAsync();

        var entries = await query
            .OrderByDescending(s => s.Id)
            .Select(s => new
            {
                s.Id,
                s.WardNo,
                s.Uuid,
                s.EnumeratorName,
                s.Username,
                s.PhoneNumber,
                s.DeviceId,
                s.SubmittedBy,
                s.Status,
                s.ValidationStatus,
                s.SubjectArea,
                s.Municipality,
                s.OdkId,
                s.OdkIndex,
                s.Version
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.WardNo = wardNo;
        ViewBag.TotalItems = totalItems;
        ViewBag.IsTrash = true;

        return View("SurveyEntries", entries);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]  
    public async Task<IActionResult> HardDeleteSurveyEntry(long id)
    {
        var exists = await _db.SurveyEntries.AnyAsync(s => s.Id == id && s.Status == "deleted");
        if (exists)
        {
            // Cascade-delete all child institution records that reference this survey_entry,
            // then delete the parent row. The FK constraint in PostgreSQL is NOT ON DELETE CASCADE
            // so we must manually remove children first across all 16 institution tables.
            foreach (var (key, fetcher) in ChildFetchers)
            {
                var child = await fetcher(_db, id);
                if (child != null)
                    _db.Remove(child);
            }
            await _db.SaveChangesAsync();

            // Now safe to hard-delete the parent
            await _db.SurveyEntries
                .Where(s => s.Id == id && s.Status == "deleted")
                .ExecuteDeleteAsync();
        }

        TempData["SuccessMessage"] = "प्रविष्टि स्थायी रूपमा मेटाइयो।";
        return RedirectToAction(nameof(SurveyEntriesTrash));
    }
    
    // ==================== HOUSEHOLDS ====================

    public async Task<IActionResult> Households(
    int? wardNo = null,
    string? settlement = null,
    string? search = null,
    int? minMembers = null,
    int? maxMembers = null,
    int page = 1,
    int pageSize = 20)
    {
        ViewData["Title"] = "घरपरिवार विवरण";

        var query = _db.Households.AsNoTracking().Where(h => h.Status != "deleted");

        // ─── Filters ───
        if (wardNo.HasValue)
            query = query.Where(h => h.WardNo == wardNo.Value);

        if (!string.IsNullOrWhiteSpace(settlement))
            query = query.Where(h => h.SettlementName != null && h.SettlementName.Contains(settlement));

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(h =>
                (h.RespondentName  != null && h.RespondentName.Contains(search))  ||
                (h.EnumeratorName  != null && h.EnumeratorName.Contains(search))  ||
                (h.RespondentPhone != null && h.RespondentPhone.Contains(search)) ||
                (h.HouseSerialNo   != null && h.HouseSerialNo.ToString().Contains(search)));

        if (minMembers.HasValue)
            query = query.Where(h => h.FamilyMemberCount >= minMembers.Value);

        if (maxMembers.HasValue)
            query = query.Where(h => h.FamilyMemberCount <= maxMembers.Value);

        var totalItems = await query.CountAsync();
        ViewBag.DeletedCount = await _db.Households.CountAsync(h => h.Status == "deleted");

        // Distinct settlements for dropdown
        ViewBag.Settlements = await _db.Households
            .Where(h => h.SettlementName != null)
            .Select(h => h.SettlementName!)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        var households = await query
            .OrderByDescending(h => h.Id)
            .Select(h => new {
                h.Id, h.WardNo, h.HouseSerialNo, h.SettlementName,
                h.RespondentName, h.RespondentPhone, h.FamilyMemberCount,
                h.EnumeratorName, h.Status, h.Municipality,
                h.RoadName, h.DistanceFromCenter, h.DeathInLastYear, h.DeathCount
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage  = page;
        ViewBag.TotalPages   = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.WardNo       = wardNo;
        ViewBag.Settlement   = settlement;
        ViewBag.Search       = search;
        ViewBag.MinMembers   = minMembers;
        ViewBag.MaxMembers   = maxMembers;
        ViewBag.TotalItems   = totalItems;

        return View("HouseHolds/Households", households);
    }
    
    [HttpGet]
    public IActionResult CreateHousehold()
    {
        ViewData["Title"] = "नयाँ घरपरिवार थप्नुहोस्";
        return View("HouseHolds/CreateHousehold");
    }

        [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateHousehold(VrindavanProfile.Web.Data.DbEntities.Household model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "नयाँ घरपरिवार थप्नुहोस्";
            return View("HouseHolds/CreateHousehold", model);
        }
        
        if (IsWardAdmin && UserWard.HasValue && model.WardNo != UserWard.Value)
        {
            ModelState.AddModelError("WardNo",
                $"तपाईं केवल वडा नं. {UserWard.Value} को डाटा प्रविष्टि गर्न सक्नुहुन्छ।");
            return View(model); // or return View("viewName", model)
        }

        // Get the real table name EF is using (usually "household" or "households")
        var entityType = _db.Model.FindEntityType(typeof(VrindavanProfile.Web.Data.DbEntities.Household));
        var tableName = entityType.GetTableName();

        var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        string sql = $@"
            INSERT INTO {tableName} (
                ward_no, house_serial_no, settlement_name, road_name, road_type, distance_from_center,
                respondent_name, respondent_phone, respondent_relation, family_member_count,
                death_in_last_year, death_count, livestock_insured, livestock_insured_count,
                enumerator_name, municipality, status, device_id, phone_number,
                submission_time, survey_start, survey_end
            ) VALUES (
                @p0, @p1, @p2, @p3, @p4, @p5,
                @p6, @p7, @p8, @p9,
                @p10, @p11, @p12, @p13,
                @p14, @p15, @p16, @p17, @p18,
                @p19, @p20, @p21
            )";

        await _db.Database.ExecuteSqlRawAsync(sql,
            model.WardNo,
            model.HouseSerialNo,
            model.SettlementName,
            model.RoadName,
            model.RoadType,
            model.DistanceFromCenter,
            model.RespondentName,
            model.RespondentPhone,
            model.RespondentRelation,
            model.FamilyMemberCount,
            model.DeathInLastYear,
            model.DeathCount,
            model.LivestockInsured,
            model.LivestockInsuredCount,
            model.EnumeratorName,
            model.Municipality,
            model.Status ?? "submitted",
            model.DeviceId,
            model.PhoneNumber,
            now,           // submission_time
            now,           // survey_start
            now            // survey_end
        );

        TempData["SuccessMessage"] = "नयाँ घरपरिवार सफलतापूर्वक थपियो।";
        return RedirectToAction(nameof(Households));
    }
    
    // ==================== EDIT HOUSEHOLD ====================

    [HttpGet]
    public async Task<IActionResult> EditHousehold(long id)
    {
        ViewData["Title"] = "घरपरिवार सम्पादन गर्नुहोस्";

        // Safe projection (avoid loading timestamptz columns)
        var data = await _db.Households
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new
            {
                h.Id,
                h.WardNo,
                h.HouseSerialNo,
                h.SettlementName,
                h.RoadName,
                h.RoadType,
                h.DistanceFromCenter,
                h.RespondentName,
                h.RespondentPhone,
                h.RespondentRelation,
                h.FamilyMemberCount,
                h.DeathInLastYear,
                h.DeathCount,
                h.LivestockInsured,
                h.LivestockInsuredCount,
                h.EnumeratorName,
                h.Municipality,
                h.Status,
                h.DeviceId,
                h.PhoneNumber
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return NotFound();
        
        if (IsWardAdmin && UserWard.HasValue && data.WardNo != UserWard.Value)
            return Forbid();

        var model = new VrindavanProfile.Web.Data.DbEntities.Household
        {
            Id = data.Id,
            WardNo = data.WardNo,
            HouseSerialNo = data.HouseSerialNo,
            SettlementName = data.SettlementName,
            RoadName = data.RoadName,
            RoadType = data.RoadType,
            DistanceFromCenter = data.DistanceFromCenter,
            RespondentName = data.RespondentName,
            RespondentPhone = data.RespondentPhone,
            RespondentRelation = data.RespondentRelation,
            FamilyMemberCount = data.FamilyMemberCount,
            DeathInLastYear = data.DeathInLastYear,
            DeathCount = data.DeathCount,
            LivestockInsured = data.LivestockInsured,
            LivestockInsuredCount = data.LivestockInsuredCount,
            EnumeratorName = data.EnumeratorName,
            Municipality = data.Municipality,
            Status = data.Status,
            DeviceId = data.DeviceId,
            PhoneNumber = data.PhoneNumber
        };

        return View("HouseHolds/EditHousehold", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditHousehold(VrindavanProfile.Web.Data.DbEntities.Household model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "घरपरिवार सम्पादन गर्नुहोस्";
            return View("HouseHolds/EditHousehold", model);
        }
        
        if (IsWardAdmin && UserWard.HasValue && model.WardNo != UserWard.Value)
        {
            ModelState.AddModelError("WardNo",
                $"तपाईं केवल वडा नं. {UserWard.Value} को डाटा परिवर्तन गर्न सक्नुहुन्छ।");
            ViewData["Title"] = "घरपरिवार सम्पादन गर्नुहोस्";
            return View("HouseHolds/EditHousehold", model);
        }

        await _db.Households
            .Where(h => h.Id == model.Id)
            .ExecuteUpdateAsync(h => h
                .SetProperty(x => x.WardNo, model.WardNo)
                .SetProperty(x => x.HouseSerialNo, model.HouseSerialNo)
                .SetProperty(x => x.SettlementName, model.SettlementName)
                .SetProperty(x => x.RoadName, model.RoadName)
                .SetProperty(x => x.RoadType, model.RoadType)
                .SetProperty(x => x.DistanceFromCenter, model.DistanceFromCenter)
                .SetProperty(x => x.RespondentName, model.RespondentName)
                .SetProperty(x => x.RespondentPhone, model.RespondentPhone)
                .SetProperty(x => x.RespondentRelation, model.RespondentRelation)
                .SetProperty(x => x.FamilyMemberCount, model.FamilyMemberCount)
                .SetProperty(x => x.DeathInLastYear, model.DeathInLastYear)
                .SetProperty(x => x.DeathCount, model.DeathCount)
                .SetProperty(x => x.LivestockInsured, model.LivestockInsured)
                .SetProperty(x => x.LivestockInsuredCount, model.LivestockInsuredCount)
                .SetProperty(x => x.EnumeratorName, model.EnumeratorName)
                .SetProperty(x => x.Municipality, model.Municipality)
                .SetProperty(x => x.Status, model.Status)
                .SetProperty(x => x.DeviceId, model.DeviceId)
                .SetProperty(x => x.PhoneNumber, model.PhoneNumber)
            );

        TempData["SuccessMessage"] = "घरपरिवार सफलतापूर्वक अपडेट गरियो।";
        return RedirectToAction(nameof(Households));
    }
    
    public async Task<IActionResult> ManageHouseholdData(long id)
    {
        ViewData["Title"] = "घरपरिवार डाटा व्यवस्थापन";

        var household = await _db.Households
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new { h.Id, h.WardNo, h.SettlementName, h.RespondentName })
            .FirstOrDefaultAsync();

        if (household == null)
            return NotFound();

        ViewBag.HouseholdId = household.Id;
        ViewBag.HouseholdInfo = $"{household.SettlementName} (वडा {household.WardNo}) - {household.RespondentName}";

        return View("HouseHolds/ManageHouseholdData");
    }
    
    // ==================== DELETE / TRASH / RESTORE / HARD DELETE ====================
    
    // GET: /Admin/HouseholdsTrash
    public async Task<IActionResult> HouseholdsTrash(int? wardNo = null, int page = 1, int pageSize = 20)
    {
        ViewData["Title"] = "घरपरिवार ट्रेस";
    
        var query = _db.Households
            .AsNoTracking()
            .Where(h => h.Status == "deleted");
    
        if (wardNo.HasValue)
            query = query.Where(h => h.WardNo == wardNo);
    
        var totalItems = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Max(1, Math.Min(page, totalPages));
    
        var households = await query
            .OrderByDescending(h => h.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(h => new {
                h.Id,
                h.WardNo,
                h.HouseSerialNo,
                h.SettlementName,
                h.RespondentName,
                h.RespondentPhone,
                h.FamilyMemberCount,
                h.EnumeratorName,
                h.Status,
                h.RoadName,
                h.DeathCount
            })
            .ToListAsync();
    
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.WardNo = wardNo;
        ViewBag.TotalItems = totalItems;
        ViewBag.IsTrash = true;
        ViewBag.DeletedCount = await _db.Households.CountAsync(h => h.Status == "deleted");
    
        return View("HouseHolds/Households", households);
    }
    
    // POST: /Admin/DeleteHousehold  (soft delete → trash)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]  
    public async Task<IActionResult> DeleteHousehold(long id)
    {
        var exists = await _db.Households
            .Where(h => h.Id == id)
            .ExecuteUpdateAsync(h => h.SetProperty(x => x.Status, "deleted"));
    
        TempData["SuccessMessage"] = "घरपरिवार ट्रेसमा सारियो।";
        return RedirectToAction(nameof(Households));
    }
    
    // POST: /Admin/RestoreHousehold  (restore from trash)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreHousehold(long id)
    {
        var restored = await _db.Households
            .Where(h => h.Id == id && h.Status == "deleted")
            .ExecuteUpdateAsync(h => h.SetProperty(x => x.Status, "submitted"));
    
        TempData["SuccessMessage"] = "घरपरिवार सफलतापूर्वक पुनर्स्थापित गरियो।";
        return RedirectToAction(nameof(HouseholdsTrash));
    }
    
    // POST: /Admin/HardDeleteHousehold  (permanent delete — with child cleanup)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]  
    public async Task<IActionResult> HardDeleteHousehold(long id)
    {
        // Delete children first (in order of dependencies)
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM family_member WHERE household_id = {id}");
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM house_detail WHERE household_id = {id}");
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM hh_death_record WHERE household_id = {id}");

        // Now delete the household itself
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM household WHERE id = {id} AND status = 'deleted'");

        TempData["SuccessMessage"] = "घरपरिवार स्थायी रूपमा मेटियो।";
        return RedirectToAction(nameof(HouseholdsTrash));
    }
    
    // ==================== HOUSEHOLD CATEGORY — BASIC ====================

    [HttpGet]
    public async Task<IActionResult> HouseholdCategoryBasic(long id)
    {
        ViewData["Title"] = "घरपरिवार — प्राथमिक विवरण";

        var data = await _db.Households
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new {
                h.Id,
                h.WardNo,
                h.HouseSerialNo,
                h.SettlementName,
                h.RoadName,
                h.RoadType,
                h.DistanceFromCenter,
                h.RespondentName,
                h.RespondentPhone,
                h.RespondentRelation,
                h.RespondentRelationOther,
                h.FamilyMemberCount,
                h.MigratedLast5Years,
                h.MigratedFrom,
                h.ResidenceType,
                h.PermanentAddress,
                h.OtherWard,
                h.CitizenshipStatus,
                h.DeathInLastYear,
                h.DeathCount,
                h.LivestockInsured,
                h.LivestockInsuredCount,
                h.NearestMarketName,
                h.EnumeratorName,
                h.Municipality,
                h.Status,
                h.DeviceId,
                h.PhoneNumber
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return NotFound();

        ViewBag.HouseholdId = data.Id;
        ViewBag.HouseholdInfo = $"{data.SettlementName} (वडा {data.WardNo}) - {data.RespondentName}";

        var model = new VrindavanProfile.Web.Data.DbEntities.Household {
            Id = data.Id, WardNo = data.WardNo, HouseSerialNo = data.HouseSerialNo,
            SettlementName = data.SettlementName, RoadName = data.RoadName,
            RoadType = data.RoadType, DistanceFromCenter = data.DistanceFromCenter,
            RespondentName = data.RespondentName, RespondentPhone = data.RespondentPhone,
            RespondentRelation = data.RespondentRelation,
            RespondentRelationOther = data.RespondentRelationOther,
            FamilyMemberCount = data.FamilyMemberCount,
            MigratedLast5Years = data.MigratedLast5Years,
            MigratedFrom = data.MigratedFrom, ResidenceType = data.ResidenceType,
            PermanentAddress = data.PermanentAddress, OtherWard = data.OtherWard,
            CitizenshipStatus = data.CitizenshipStatus,
            DeathInLastYear = data.DeathInLastYear, DeathCount = data.DeathCount,
            LivestockInsured = data.LivestockInsured,
            LivestockInsuredCount = data.LivestockInsuredCount,
            NearestMarketName = data.NearestMarketName,
            EnumeratorName = data.EnumeratorName, Municipality = data.Municipality,
            Status = data.Status, DeviceId = data.DeviceId, PhoneNumber = data.PhoneNumber
        };

        return View("HouseHolds/HouseholdCategoryBasic", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HouseholdCategoryBasic(VrindavanProfile.Web.Data.DbEntities.Household model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "घरपरिवार — प्राथमिक विवरण";
            return View("HouseHolds/HouseholdCategoryBasic", model);
        }

        await _db.Households
            .Where(h => h.Id == model.Id)
            .ExecuteUpdateAsync(h => h
                .SetProperty(x => x.WardNo, model.WardNo)
                .SetProperty(x => x.HouseSerialNo, model.HouseSerialNo)
                .SetProperty(x => x.SettlementName, model.SettlementName)
                .SetProperty(x => x.RoadName, model.RoadName)
                .SetProperty(x => x.RoadType, model.RoadType)
                .SetProperty(x => x.DistanceFromCenter, model.DistanceFromCenter)
                .SetProperty(x => x.RespondentName, model.RespondentName)
                .SetProperty(x => x.RespondentPhone, model.RespondentPhone)
                .SetProperty(x => x.RespondentRelation, model.RespondentRelation)
                .SetProperty(x => x.RespondentRelationOther, model.RespondentRelationOther)
                .SetProperty(x => x.FamilyMemberCount, model.FamilyMemberCount)
                .SetProperty(x => x.MigratedLast5Years, model.MigratedLast5Years)
                .SetProperty(x => x.MigratedFrom, model.MigratedFrom)
                .SetProperty(x => x.ResidenceType, model.ResidenceType)
                .SetProperty(x => x.PermanentAddress, model.PermanentAddress)
                .SetProperty(x => x.OtherWard, model.OtherWard)
                .SetProperty(x => x.CitizenshipStatus, model.CitizenshipStatus)
                .SetProperty(x => x.DeathInLastYear, model.DeathInLastYear)
                .SetProperty(x => x.DeathCount, model.DeathCount)
                .SetProperty(x => x.LivestockInsured, model.LivestockInsured)
                .SetProperty(x => x.LivestockInsuredCount, model.LivestockInsuredCount)
                .SetProperty(x => x.NearestMarketName, model.NearestMarketName)
                .SetProperty(x => x.EnumeratorName, model.EnumeratorName)
                .SetProperty(x => x.Municipality, model.Municipality)
                .SetProperty(x => x.Status, model.Status)
                .SetProperty(x => x.DeviceId, model.DeviceId)
                .SetProperty(x => x.PhoneNumber, model.PhoneNumber)
            );

        TempData["SuccessMessage"] = "प्राथमिक विवरण सफलतापूर्वक अपडेट गरियो।";
        return RedirectToAction(nameof(ManageHouseholdData), new { id = model.Id });
    }
    
    // ==================== HOUSEHOLD CATEGORY: LOCATION ====================

    [HttpGet]
    public async Task<IActionResult> HouseholdCategoryLocation(long id)
    {
        ViewData["Title"] = "घरपरिवार स्थान र GPS विवरण";

        var data = await _db.Households
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new
            {
                h.Id,
                h.WardNo,
                h.SettlementName,
                h.RespondentName,
                h.HouseSerialNo,
                h.RoadName,
                h.RoadType,
                h.DistanceFromCenter,
                h.Latitude,
                h.Longitude,
                h.Altitude,
                h.GpsPrecision,
                h.GeoPoint,
                h.Photo,
                h.PhotoUrl
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return NotFound();

        var model = new VrindavanProfile.Web.Data.DbEntities.Household
        {
            Id = data.Id,
            WardNo = data.WardNo,
            SettlementName = data.SettlementName,
            RespondentName = data.RespondentName,
            HouseSerialNo = data.HouseSerialNo,
            RoadName = data.RoadName,
            RoadType = data.RoadType,
            DistanceFromCenter = data.DistanceFromCenter,
            Latitude = data.Latitude,
            Longitude = data.Longitude,
            Altitude = data.Altitude,
            GpsPrecision = data.GpsPrecision,
            GeoPoint = data.GeoPoint,
            Photo = data.Photo,
            PhotoUrl = data.PhotoUrl
        };

        return View("HouseHolds/HouseholdCategoryLocation", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HouseholdCategoryLocation(VrindavanProfile.Web.Data.DbEntities.Household model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "घरपरिवार स्थान र GPS विवरण";
            return View("HouseHolds/HouseholdCategoryLocation", model);
        }

        await _db.Households
            .Where(h => h.Id == model.Id)
            .ExecuteUpdateAsync(h => h
                .SetProperty(x => x.Latitude, model.Latitude)
                .SetProperty(x => x.Longitude, model.Longitude)
                .SetProperty(x => x.Altitude, model.Altitude)
                .SetProperty(x => x.GpsPrecision, model.GpsPrecision)
                .SetProperty(x => x.GeoPoint, model.GeoPoint)
                .SetProperty(x => x.Photo, model.Photo)
                .SetProperty(x => x.PhotoUrl, model.PhotoUrl)
            );

        TempData["SuccessMessage"] = "स्थान र GPS विवरण सफलतापूर्वक अपडेट गरियो।";
        return RedirectToAction(nameof(ManageHouseholdData), new { id = model.Id });
    }
    
    // ==================== HOUSEHOLD CATEGORY: FACILITIES ====================
    
    [HttpGet]
    public async Task<IActionResult> HouseholdCategoryFacilities(long id)
    {
        ViewData["Title"] = "घरपरिवार सुविधाहरू र सेवा";
    
        var data = await _db.Households
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new
            {
                h.Id,
                h.WardNo,
                h.SettlementName,
                h.RespondentName,
                h.DrinkingWaterSource,
                h.DrinkingWaterOther,
                h.CookingFuelPrimary,
                h.CookingFuelSecondary,
                h.StoveType,
                h.LightSource,
                h.LightSourceOther,
                h.WasteManagement,
                h.ToiletType,
                h.MarketTravelTime,
                h.FirstHealthVisit,
                h.HospitalTravelTime,
                h.SchoolTravelTime,
                h.ChildLabor,
                h.SchoolTypeGovt,
                h.SchoolTypePrivate,
                h.SchoolTypeMadrasa,
                h.SchoolTypeSummary
            })
            .FirstOrDefaultAsync();
    
        if (data == null)
            return NotFound();
    
        var model = new VrindavanProfile.Web.Data.DbEntities.Household
        {
            Id = data.Id,
            WardNo = data.WardNo,
            SettlementName = data.SettlementName,
            RespondentName = data.RespondentName,
            DrinkingWaterSource = data.DrinkingWaterSource,
            DrinkingWaterOther = data.DrinkingWaterOther,
            CookingFuelPrimary = data.CookingFuelPrimary,
            CookingFuelSecondary = data.CookingFuelSecondary,
            StoveType = data.StoveType,
            LightSource = data.LightSource,
            LightSourceOther = data.LightSourceOther,
            WasteManagement = data.WasteManagement,
            ToiletType = data.ToiletType,
            MarketTravelTime = data.MarketTravelTime,
            FirstHealthVisit = data.FirstHealthVisit,
            HospitalTravelTime = data.HospitalTravelTime,
            SchoolTravelTime = data.SchoolTravelTime,
            ChildLabor = data.ChildLabor,
            SchoolTypeGovt = data.SchoolTypeGovt,
            SchoolTypePrivate = data.SchoolTypePrivate,
            SchoolTypeMadrasa = data.SchoolTypeMadrasa,
            SchoolTypeSummary = data.SchoolTypeSummary
        };
    
        return View("HouseHolds/HouseholdCategoryFacilities", model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HouseholdCategoryFacilities(VrindavanProfile.Web.Data.DbEntities.Household model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "घरपरिवार सुविधाहरू र सेवा";
            return View("HouseHolds/HouseholdCategoryFacilities", model);
        }
    
        await _db.Households
            .Where(h => h.Id == model.Id)
            .ExecuteUpdateAsync(h => h
                .SetProperty(x => x.DrinkingWaterSource, model.DrinkingWaterSource)
                .SetProperty(x => x.DrinkingWaterOther, model.DrinkingWaterOther)
                .SetProperty(x => x.CookingFuelPrimary, model.CookingFuelPrimary)
                .SetProperty(x => x.CookingFuelSecondary, model.CookingFuelSecondary)
                .SetProperty(x => x.StoveType, model.StoveType)
                .SetProperty(x => x.LightSource, model.LightSource)
                .SetProperty(x => x.LightSourceOther, model.LightSourceOther)
                .SetProperty(x => x.WasteManagement, model.WasteManagement)
                .SetProperty(x => x.ToiletType, model.ToiletType)
                .SetProperty(x => x.MarketTravelTime, model.MarketTravelTime)
                .SetProperty(x => x.FirstHealthVisit, model.FirstHealthVisit)
                .SetProperty(x => x.HospitalTravelTime, model.HospitalTravelTime)
                .SetProperty(x => x.SchoolTravelTime, model.SchoolTravelTime)
                .SetProperty(x => x.ChildLabor, model.ChildLabor)
                .SetProperty(x => x.SchoolTypeGovt, model.SchoolTypeGovt)
                .SetProperty(x => x.SchoolTypePrivate, model.SchoolTypePrivate)
                .SetProperty(x => x.SchoolTypeMadrasa, model.SchoolTypeMadrasa)
                .SetProperty(x => x.SchoolTypeSummary, model.SchoolTypeSummary)
            );
    
        TempData["SuccessMessage"] = "सुविधाहरू र सेवा विवरण सफलतापूर्वक अपडेट गरियो।";
        return RedirectToAction(nameof(ManageHouseholdData), new { id = model.Id });
    }
    
    // ==================== HOUSEHOLD CATEGORY: ASSETS ====================

    [HttpGet]
    public async Task<IActionResult> HouseholdCategoryAssets(long id)
    {
        ViewData["Title"] = "घरपरिवार संपत्ति";

        var data = await _db.Households
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new
            {
                h.Id,
                h.WardNo,
                h.SettlementName,
                h.RespondentName,
                h.AssetsSummary,
                h.AssetRadio,
                h.AssetBicycle,
                h.AssetTelevision,
                h.AssetPhone,
                h.AssetComputer,
                h.AssetInternet,
                h.AssetMotorcycle,
                h.AssetCar,
                h.AssetRefrigerator,
                h.AssetTractor,
                h.AssetBusTruck,
                h.AssetTipperTruck,
                h.AssetExcavator,
                h.AssetNone
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return NotFound();

        var model = new VrindavanProfile.Web.Data.DbEntities.Household
        {
            Id = data.Id,
            WardNo = data.WardNo,
            SettlementName = data.SettlementName,
            RespondentName = data.RespondentName,
            AssetsSummary = data.AssetsSummary,
            AssetRadio = data.AssetRadio,
            AssetBicycle = data.AssetBicycle,
            AssetTelevision = data.AssetTelevision,
            AssetPhone = data.AssetPhone,
            AssetComputer = data.AssetComputer,
            AssetInternet = data.AssetInternet,
            AssetMotorcycle = data.AssetMotorcycle,
            AssetCar = data.AssetCar,
            AssetRefrigerator = data.AssetRefrigerator,
            AssetTractor = data.AssetTractor,
            AssetBusTruck = data.AssetBusTruck,
            AssetTipperTruck = data.AssetTipperTruck,
            AssetExcavator = data.AssetExcavator,
            AssetNone = data.AssetNone
        };

        return View("HouseHolds/HouseholdCategoryAssets", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HouseholdCategoryAssets(VrindavanProfile.Web.Data.DbEntities.Household model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "घरपरिवार संपत्ति";
            return View("HouseHolds/HouseholdCategoryAssets", model);
        }

        await _db.Households
            .Where(h => h.Id == model.Id)
            .ExecuteUpdateAsync(h => h
                .SetProperty(x => x.AssetRadio, model.AssetRadio)
                .SetProperty(x => x.AssetBicycle, model.AssetBicycle)
                .SetProperty(x => x.AssetTelevision, model.AssetTelevision)
                .SetProperty(x => x.AssetPhone, model.AssetPhone)
                .SetProperty(x => x.AssetComputer, model.AssetComputer)
                .SetProperty(x => x.AssetInternet, model.AssetInternet)
                .SetProperty(x => x.AssetMotorcycle, model.AssetMotorcycle)
                .SetProperty(x => x.AssetCar, model.AssetCar)
                .SetProperty(x => x.AssetRefrigerator, model.AssetRefrigerator)
                .SetProperty(x => x.AssetTractor, model.AssetTractor)
                .SetProperty(x => x.AssetBusTruck, model.AssetBusTruck)
                .SetProperty(x => x.AssetTipperTruck, model.AssetTipperTruck)
                .SetProperty(x => x.AssetExcavator, model.AssetExcavator)
                .SetProperty(x => x.AssetNone, model.AssetNone)
                .SetProperty(x => x.AssetsSummary, model.AssetsSummary)
            );

        TempData["SuccessMessage"] = "संपत्ति विवरण सफलतापूर्वक अपडेट गरियो।";
        return RedirectToAction(nameof(ManageHouseholdData), new { id = model.Id });
    }
    
    // ==================== HOUSEHOLD CATEGORY: OTHER ====================

    [HttpGet]
    public async Task<IActionResult> HouseholdCategoryOther(long id)
    {
        ViewData["Title"] = "घर, विपद र जोखिम विवरण";

        var data = await _db.Households
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new
            {
                h.Id,
                h.WardNo,
                h.SettlementName,
                h.RespondentName,
                h.HouseInfoNote,
                h.HouseCount,
                h.EarthquakeResistant,
                h.DisasterDamaged,
                h.ReceivedSubsidy,
                h.SubsidyInstallments,
                h.HouseRebuilt,
                h.HasFamilyRiskPlan,
                h.DisasterRisk,
                h.DisasterRiskSummary,
                h.RiskEarthquake,
                h.RiskFlood,
                h.RiskLandslide,
                h.RiskLightning,
                h.RiskStorm,
                h.RiskColdWave,
                h.RiskInundation,
                h.ClosingNote,
                h.DistanceFromCenter2
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return NotFound();

        var model = new VrindavanProfile.Web.Data.DbEntities.Household
        {
            Id = data.Id,
            WardNo = data.WardNo,
            SettlementName = data.SettlementName,
            RespondentName = data.RespondentName,
            HouseInfoNote = data.HouseInfoNote,
            HouseCount = data.HouseCount,
            EarthquakeResistant = data.EarthquakeResistant,
            DisasterDamaged = data.DisasterDamaged,
            ReceivedSubsidy = data.ReceivedSubsidy,
            SubsidyInstallments = data.SubsidyInstallments,
            HouseRebuilt = data.HouseRebuilt,
            HasFamilyRiskPlan = data.HasFamilyRiskPlan,
            DisasterRisk = data.DisasterRisk,
            DisasterRiskSummary = data.DisasterRiskSummary,
            RiskEarthquake = data.RiskEarthquake,
            RiskFlood = data.RiskFlood,
            RiskLandslide = data.RiskLandslide,
            RiskLightning = data.RiskLightning,
            RiskStorm = data.RiskStorm,
            RiskColdWave = data.RiskColdWave,
            RiskInundation = data.RiskInundation,
            ClosingNote = data.ClosingNote,
            DistanceFromCenter2 = data.DistanceFromCenter2
        };

        return View("HouseHolds/HouseholdCategoryOther", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HouseholdCategoryOther(VrindavanProfile.Web.Data.DbEntities.Household model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "घर, विपद र जोखिम विवरण";
            return View("HouseHolds/HouseholdCategoryOther", model);
        }

        await _db.Households
            .Where(h => h.Id == model.Id)
            .ExecuteUpdateAsync(h => h
                .SetProperty(x => x.HouseCount, model.HouseCount)
                .SetProperty(x => x.EarthquakeResistant, model.EarthquakeResistant)
                .SetProperty(x => x.DisasterDamaged, model.DisasterDamaged)
                .SetProperty(x => x.ReceivedSubsidy, model.ReceivedSubsidy)
                .SetProperty(x => x.SubsidyInstallments, model.SubsidyInstallments)
                .SetProperty(x => x.HouseRebuilt, model.HouseRebuilt)
                .SetProperty(x => x.HasFamilyRiskPlan, model.HasFamilyRiskPlan)
                .SetProperty(x => x.DisasterRisk, model.DisasterRisk)
                .SetProperty(x => x.DisasterRiskSummary, model.DisasterRiskSummary)
                .SetProperty(x => x.RiskEarthquake, model.RiskEarthquake)
                .SetProperty(x => x.RiskFlood, model.RiskFlood)
                .SetProperty(x => x.RiskLandslide, model.RiskLandslide)
                .SetProperty(x => x.RiskLightning, model.RiskLightning)
                .SetProperty(x => x.RiskStorm, model.RiskStorm)
                .SetProperty(x => x.RiskColdWave, model.RiskColdWave)
                .SetProperty(x => x.RiskInundation, model.RiskInundation)
                .SetProperty(x => x.ClosingNote, model.ClosingNote)
                .SetProperty(x => x.DistanceFromCenter2, model.DistanceFromCenter2)
            );

        TempData["SuccessMessage"] = "घर, विपद र जोखिम विवरण सफलतापूर्वक अपडेट गरियो।";
        return RedirectToAction(nameof(ManageHouseholdData), new { id = model.Id });
    }
    
        // ==================== HOUSEHOLD CATEGORY AGRICULTURE ====================

    [HttpGet]
    public async Task<IActionResult> HouseholdCategoryAgriculture(long id)
    {
        ViewData["Title"] = "कृषि तथा पशुपालन";
        ViewData["BreadcrumbItems"] = new List<(string Label, string? Url)> {
            ("घरपरिवार सूची", "/Admin/Households"),
            ("घरपरिवार व्यवस्थापन", $"/Admin/ManageHouseholdData/{id}")
        };

        var household = await _db.Households
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new {
                h.Id,
                h.WardNo,
                h.SettlementName,
                h.RoadName,
                h.RoadType,
                h.DistanceFromCenter,
                h.RespondentName,
                // Land
                h.HasAgricultureLand,
                h.OwnLandKattha,
                h.OthersLandKattha,
                h.OtherOwnershipKattha,
                h.KhetAreaKattha,
                h.BariAreaKattha,
                h.FallowLandKattha,
                h.IrrigationStatus,
                h.IrrigationMediumSummary,
                h.IrrigationCanal,
                h.IrrigationTubewell,
                h.IrrigationElectric,
                h.FarmingMethod,
                // Cereals
                h.HasCerealCrops,
                h.CerealCropSummary,
                h.CerealRice,
                h.CerealMaize,
                h.CerealWheat,
                h.CerealBarley,
                h.RiceArea,
                h.RiceProductionKg,
                h.RiceSalesRupees,
                h.RiceDeficitRupees,
                h.MaizeArea,
                h.MaizeProductionKg,
                h.MaizeSalesRupees,
                h.MaizeDeficitRupees,
                h.WheatArea,
                h.WheatProductionKg,
                h.WheatSalesRupees,
                h.WheatDeficitRupees,
                h.BarleyArea,
                h.BarleyProductionKg,
                h.BarleySalesRupees,
                h.BarleyDeficitRupees,
                // Pulses
                h.HasPulseCrops,
                h.PulseCropSummary,
                h.PulseBlackDal,
                h.PulseRahar,
                h.PulseMasuro,
                h.PulseChana,
                h.PulseSimi,
                h.PulseBhattmas,
                h.PulseMung,
                h.PulseKhesari,
                h.Pulse1Area,
                h.Pulse1ProductionKg,
                h.Pulse1SalesRupees,
                h.Pulse1DeficitRupees,
                h.Pulse2Area,
                h.Pulse2ProductionKg,
                h.Pulse2SalesRupees,
                h.Pulse2DeficitRupees,
                h.Pulse3Area,
                h.Pulse3ProductionKg,
                h.Pulse3SalesRupees,
                h.Pulse3DeficitRupees,
                h.Pulse4Area,
                h.Pulse4ProductionKg,
                h.Pulse4SalesRupees,
                h.Pulse4DeficitRupees,
                h.Pulse5Area,
                h.Pulse5ProductionKg,
                h.Pulse5SalesRupees,
                h.Pulse5DeficitRupees,
                h.Pulse6Area,
                h.Pulse6ProductionKg,
                h.Pulse6SalesRupees,
                h.Pulse6DeficitRupees,
                h.Pulse7Area,
                h.Pulse7ProductionKg,
                h.Pulse7SalesRupees,
                h.Pulse7DeficitRupees,
                h.Pulse8Area,
                h.Pulse8ProductionKg,
                h.Pulse8SalesRupees,
                h.Pulse8DeficitRupees,
                // Oilseeds
                h.HasOilseedCrops,
                h.OilseedCropSummary,
                h.OilseedTori,
                h.OilseedTil,
                h.OilseedSunflower,
                h.OilseedOther,
                h.Oilseed1Area,
                h.Oilseed1ProductionKg,
                h.Oilseed1SalesRupees,
                h.Oilseed1DeficitRupees,
                h.Oilseed2Area,
                h.Oilseed2ProductionKg,
                h.Oilseed2SalesRupees,
                h.Oilseed2DeficitRupees,
                h.Oilseed3Area,
                h.Oilseed3ProductionKg,
                h.Oilseed3SalesRupees,
                h.Oilseed3DeficitRupees,
                h.Oilseed4Area,
                h.Oilseed4ProductionKg,
                h.Oilseed4SalesRupees,
                h.Oilseed4DeficitRupees,
                // Cash crops
                h.HasCashCrops,
                h.CashCropSummary,
                h.CashGingerTurmeric,
                h.CashOnionGarlic,
                h.CashMushroom,
                h.CashSugarcane,
                h.CashChili,
                h.CashPeanut,
                h.Cash1Area,
                h.Cash1ProductionKg,
                h.Cash1SalesRupees,
                h.Cash1DeficitRupees,
                h.Cash2Area,
                h.Cash2ProductionKg,
                h.Cash2SalesRupees,
                h.Cash2DeficitRupees,
                h.Cash3Area,
                h.Cash3ProductionKg,
                h.Cash3SalesRupees,
                h.Cash3DeficitRupees,
                h.Cash4Area,
                h.Cash4ProductionKg,
                h.Cash4SalesRupees,
                h.Cash4DeficitRupees,
                h.Cash5Area,
                h.Cash5ProductionKg,
                h.Cash5SalesRupees,
                h.Cash5DeficitRupees,
                h.Cash6Area,
                h.Cash6ProductionKg,
                h.Cash6SalesRupees,
                h.Cash6DeficitRupees,
                // Fruits
                h.HasFruitCrops,
                h.FruitCropSummary,
                h.FruitMango,
                h.FruitLemon,
                h.FruitPomelo,
                h.FruitPomegranate,
                h.FruitBanana,
                h.FruitAmba,
                h.FruitLichi,
                h.FruitSarifa,
                h.Fruit1Area,
                h.Fruit1ProductionKg,
                h.Fruit1SalesRupees,
                h.Fruit1DeficitRupees,
                h.Fruit2Area,
                h.Fruit2ProductionKg,
                h.Fruit2SalesRupees,
                h.Fruit2DeficitRupees,
                h.Fruit3Area,
                h.Fruit3ProductionKg,
                h.Fruit3SalesRupees,
                h.Fruit3DeficitRupees,
                h.Fruit4Area,
                h.Fruit4ProductionKg,
                h.Fruit4SalesRupees,
                h.Fruit4DeficitRupees,
                h.Fruit5Area,
                h.Fruit5ProductionKg,
                h.Fruit5SalesRupees,
                h.Fruit5DeficitRupees,
                h.Fruit6Area,
                h.Fruit6ProductionKg,
                h.Fruit6SalesRupees,
                h.Fruit6DeficitRupees,
                h.Fruit7Area,
                h.Fruit7ProductionKg,
                h.Fruit7SalesRupees,
                h.Fruit7DeficitRupees,
                h.Fruit8Area,
                h.Fruit8ProductionKg,
                h.Fruit8SalesRupees,
                h.Fruit8DeficitRupees,
                // Herbs
                h.HasHerbCrops,
                h.HerbCropSummary,
                h.HerbHarroBarro,
                h.HerbGurjo,
                h.HerbTulsi,
                h.HerbAmla,
                h.HerbSarpagandha,
                h.HerbAjwain,
                h.HerbSutli,
                h.HerbAloeVera,
                h.HerbBojho,
                h.Herb1Area,
                h.Herb1ProductionKg,
                h.Herb1SalesRupees,
                h.Herb2Area,
                h.Herb2ProductionKg,
                h.Herb2SalesRupees,
                h.Herb3Area,
                h.Herb3ProductionKg,
                h.Herb3SalesRupees,
                h.Herb4Area,
                h.Herb4ProductionKg,
                h.Herb4SalesRupees,
                h.Herb5Area,
                h.Herb5ProductionKg,
                h.Herb5SalesRupees,
                h.Herb6Area,
                h.Herb6ProductionKg,
                h.Herb6SalesRupees,
                h.Herb7Area,
                h.Herb7ProductionKg,
                h.Herb7SalesRupees,
                h.Herb8Area,
                h.Herb8ProductionKg,
                h.Herb8SalesRupees,
                h.Herb9Area,
                h.Herb9ProductionKg,
                h.Herb9SalesRupees,
                // Livestock
                h.HasLivestock,
                h.DairySummary,
                h.DairyCow,
                h.DairyBuffalo,
                h.DairyOther,
                h.CowCount,
                h.CowMilkDaily,
                h.CowMilkSold,
                h.CowCurdSold,
                h.CowAnnualIncome,
                h.BuffaloCount,
                h.BuffaloMilkDaily,
                h.BuffaloMilkSold,
                h.BuffaloCurdSold,
                h.BuffaloAnnualIncome,
                h.OtherDairyCount,
                h.OtherDairyMilkDaily,
                h.OtherDairyMilkSold,
                h.OtherDairyCurdSold,
                h.OtherDairyAnnualIncome,
                h.GheeSold,
                h.GheeSold2,
                h.GheeSold3,
                // Meat
                h.MeatSummary,
                h.MeatGoatSheep,
                h.MeatCalf,
                h.MeatPig,
                h.MeatOther,
                h.GoatCount,
                h.GoatAnnualSold,
                h.GoatAnnualIncome,
                h.CalfCount,
                h.CalfAnnualSold,
                h.CalfAnnualIncome,
                h.PigCount,
                h.PigAnnualSold,
                h.PigAnnualIncome,
                h.OtherMeatCount,
                h.OtherMeatAnnualSold,
                h.OtherMeatAnnualIncome,
                // Poultry
                h.PoultrySummary,
                h.PoultryChicken,
                h.PoultryDuck,
                h.PoultryPigeon,
                h.PoultryBattai,
                h.PoultryCount,
                h.PoultryEggAnnual,
                h.PoultryMeatSoldKg,
                h.PoultryAnnualIncome,
                // Fish
                h.HasFishFarming,
                h.FishPondCount,
                h.FishPondArea,
                h.FishProductionKg,
                h.FishAnnualIncome,
                h.FishSubsidy,
                // Beekeeping
                h.HasBeekeeping,
                h.BeeHiveCount,
                h.HoneyProductionKg,
                h.HoneyAnnualIncome,
                h.BeeSubsidy,
                // Income
                h.IncomeVegetables,
                h.IncomeLabor,
                h.IncomeTrade,
                h.IncomeForeign,
                h.IncomeJob,
                h.IncomeOther,
                // Expenditure
                h.ExpenseFood,
                h.ExpenseClothes,
                h.ExpenseEducation,
                h.ExpenseHealth,
                h.ExpenseFestivals,
                h.ExpenseAgriculture,
                h.ExpenseFuel,
                h.ExpenseMobileInternet,
                h.FoodSufficiencyMonths,
                // Deficit spending
                h.DeficitSpendingSummary,
                h.DeficitJob,
                h.DeficitDailyWage,
                h.DeficitForeignJob,
                h.DeficitTrade,
                h.DeficitLoan,
                // Banking
                h.HasBankAccount,
                h.BankAccountSummary,
                h.BankAccountBank,
                h.BankAccountCoop,
                h.BankAccountSavings,
                h.BankAccountMoneylender,
                h.BankAccountOther,
                h.HasLoan,
                h.LoanAmount,
                h.LoanSourceSummary,
                h.LoanFromBank,
                h.LoanFromCoop,
                h.LoanFromMoneylender,
                h.LoanFromRelatives,
                h.LoanType,
                h.LoanPurposeSummary,
                h.LoanPurposeBusiness,
                h.LoanPurposeHouse,
                h.LoanPurposeMedical,
                h.LoanPurposeEducation,
                h.LoanPurposeOther,
                h.LoanPurposeOtherDetail
            })
            .FirstOrDefaultAsync();

        if (household == null)
            return NotFound();

        ViewBag.HouseholdId = household.Id;
        ViewBag.HouseholdInfo = $"वडा {household.WardNo} — {household.SettlementName}";
        return View("HouseHolds/HouseholdCategoryAgriculture", household);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HouseholdCategoryAgriculture(VrindavanProfile.Web.Data.DbEntities.Household model)
    {
        if (!ModelState.IsValid)
        {
            return await HouseholdCategoryAgriculture(model.Id);
        }

        await _db.Households
            .Where(h => h.Id == model.Id)
            .ExecuteUpdateAsync(h => h
                // Land
                .SetProperty(x => x.HasAgricultureLand, model.HasAgricultureLand)
                .SetProperty(x => x.OwnLandKattha, model.OwnLandKattha)
                .SetProperty(x => x.OthersLandKattha, model.OthersLandKattha)
                .SetProperty(x => x.OtherOwnershipKattha, model.OtherOwnershipKattha)
                .SetProperty(x => x.KhetAreaKattha, model.KhetAreaKattha)
                .SetProperty(x => x.BariAreaKattha, model.BariAreaKattha)
                .SetProperty(x => x.FallowLandKattha, model.FallowLandKattha)
                .SetProperty(x => x.IrrigationStatus, model.IrrigationStatus)
                .SetProperty(x => x.IrrigationMediumSummary, model.IrrigationMediumSummary)
                .SetProperty(x => x.IrrigationCanal, model.IrrigationCanal)
                .SetProperty(x => x.IrrigationTubewell, model.IrrigationTubewell)
                .SetProperty(x => x.IrrigationElectric, model.IrrigationElectric)
                .SetProperty(x => x.FarmingMethod, model.FarmingMethod)
                // Cereals
                .SetProperty(x => x.HasCerealCrops, model.HasCerealCrops)
                .SetProperty(x => x.CerealCropSummary, model.CerealCropSummary)
                .SetProperty(x => x.CerealRice, model.CerealRice)
                .SetProperty(x => x.CerealMaize, model.CerealMaize)
                .SetProperty(x => x.CerealWheat, model.CerealWheat)
                .SetProperty(x => x.CerealBarley, model.CerealBarley)
                .SetProperty(x => x.RiceArea, model.RiceArea)
                .SetProperty(x => x.RiceProductionKg, model.RiceProductionKg)
                .SetProperty(x => x.RiceSalesRupees, model.RiceSalesRupees)
                .SetProperty(x => x.RiceDeficitRupees, model.RiceDeficitRupees)
                .SetProperty(x => x.MaizeArea, model.MaizeArea)
                .SetProperty(x => x.MaizeProductionKg, model.MaizeProductionKg)
                .SetProperty(x => x.MaizeSalesRupees, model.MaizeSalesRupees)
                .SetProperty(x => x.MaizeDeficitRupees, model.MaizeDeficitRupees)
                .SetProperty(x => x.WheatArea, model.WheatArea)
                .SetProperty(x => x.WheatProductionKg, model.WheatProductionKg)
                .SetProperty(x => x.WheatSalesRupees, model.WheatSalesRupees)
                .SetProperty(x => x.WheatDeficitRupees, model.WheatDeficitRupees)
                .SetProperty(x => x.BarleyArea, model.BarleyArea)
                .SetProperty(x => x.BarleyProductionKg, model.BarleyProductionKg)
                .SetProperty(x => x.BarleySalesRupees, model.BarleySalesRupees)
                .SetProperty(x => x.BarleyDeficitRupees, model.BarleyDeficitRupees)
                // Pulses
                .SetProperty(x => x.HasPulseCrops, model.HasPulseCrops)
                .SetProperty(x => x.PulseCropSummary, model.PulseCropSummary)
                .SetProperty(x => x.PulseBlackDal, model.PulseBlackDal)
                .SetProperty(x => x.PulseRahar, model.PulseRahar)
                .SetProperty(x => x.PulseMasuro, model.PulseMasuro)
                .SetProperty(x => x.PulseChana, model.PulseChana)
                .SetProperty(x => x.PulseSimi, model.PulseSimi)
                .SetProperty(x => x.PulseBhattmas, model.PulseBhattmas)
                .SetProperty(x => x.PulseMung, model.PulseMung)
                .SetProperty(x => x.PulseKhesari, model.PulseKhesari)
                .SetProperty(x => x.Pulse1Area, model.Pulse1Area)
                .SetProperty(x => x.Pulse1ProductionKg, model.Pulse1ProductionKg)
                .SetProperty(x => x.Pulse1SalesRupees, model.Pulse1SalesRupees)
                .SetProperty(x => x.Pulse1DeficitRupees, model.Pulse1DeficitRupees)
                .SetProperty(x => x.Pulse2Area, model.Pulse2Area)
                .SetProperty(x => x.Pulse2ProductionKg, model.Pulse2ProductionKg)
                .SetProperty(x => x.Pulse2SalesRupees, model.Pulse2SalesRupees)
                .SetProperty(x => x.Pulse2DeficitRupees, model.Pulse2DeficitRupees)
                .SetProperty(x => x.Pulse3Area, model.Pulse3Area)
                .SetProperty(x => x.Pulse3ProductionKg, model.Pulse3ProductionKg)
                .SetProperty(x => x.Pulse3SalesRupees, model.Pulse3SalesRupees)
                .SetProperty(x => x.Pulse3DeficitRupees, model.Pulse3DeficitRupees)
                .SetProperty(x => x.Pulse4Area, model.Pulse4Area)
                .SetProperty(x => x.Pulse4ProductionKg, model.Pulse4ProductionKg)
                .SetProperty(x => x.Pulse4SalesRupees, model.Pulse4SalesRupees)
                .SetProperty(x => x.Pulse4DeficitRupees, model.Pulse4DeficitRupees)
                .SetProperty(x => x.Pulse5Area, model.Pulse5Area)
                .SetProperty(x => x.Pulse5ProductionKg, model.Pulse5ProductionKg)
                .SetProperty(x => x.Pulse5SalesRupees, model.Pulse5SalesRupees)
                .SetProperty(x => x.Pulse5DeficitRupees, model.Pulse5DeficitRupees)
                .SetProperty(x => x.Pulse6Area, model.Pulse6Area)
                .SetProperty(x => x.Pulse6ProductionKg, model.Pulse6ProductionKg)
                .SetProperty(x => x.Pulse6SalesRupees, model.Pulse6SalesRupees)
                .SetProperty(x => x.Pulse6DeficitRupees, model.Pulse6DeficitRupees)
                .SetProperty(x => x.Pulse7Area, model.Pulse7Area)
                .SetProperty(x => x.Pulse7ProductionKg, model.Pulse7ProductionKg)
                .SetProperty(x => x.Pulse7SalesRupees, model.Pulse7SalesRupees)
                .SetProperty(x => x.Pulse7DeficitRupees, model.Pulse7DeficitRupees)
                .SetProperty(x => x.Pulse8Area, model.Pulse8Area)
                .SetProperty(x => x.Pulse8ProductionKg, model.Pulse8ProductionKg)
                .SetProperty(x => x.Pulse8SalesRupees, model.Pulse8SalesRupees)
                .SetProperty(x => x.Pulse8DeficitRupees, model.Pulse8DeficitRupees)
                // Oilseeds
                .SetProperty(x => x.HasOilseedCrops, model.HasOilseedCrops)
                .SetProperty(x => x.OilseedCropSummary, model.OilseedCropSummary)
                .SetProperty(x => x.OilseedTori, model.OilseedTori)
                .SetProperty(x => x.OilseedTil, model.OilseedTil)
                .SetProperty(x => x.OilseedSunflower, model.OilseedSunflower)
                .SetProperty(x => x.OilseedOther, model.OilseedOther)
                .SetProperty(x => x.Oilseed1Area, model.Oilseed1Area)
                .SetProperty(x => x.Oilseed1ProductionKg, model.Oilseed1ProductionKg)
                .SetProperty(x => x.Oilseed1SalesRupees, model.Oilseed1SalesRupees)
                .SetProperty(x => x.Oilseed1DeficitRupees, model.Oilseed1DeficitRupees)
                .SetProperty(x => x.Oilseed2Area, model.Oilseed2Area)
                .SetProperty(x => x.Oilseed2ProductionKg, model.Oilseed2ProductionKg)
                .SetProperty(x => x.Oilseed2SalesRupees, model.Oilseed2SalesRupees)
                .SetProperty(x => x.Oilseed2DeficitRupees, model.Oilseed2DeficitRupees)
                .SetProperty(x => x.Oilseed3Area, model.Oilseed3Area)
                .SetProperty(x => x.Oilseed3ProductionKg, model.Oilseed3ProductionKg)
                .SetProperty(x => x.Oilseed3SalesRupees, model.Oilseed3SalesRupees)
                .SetProperty(x => x.Oilseed3DeficitRupees, model.Oilseed3DeficitRupees)
                .SetProperty(x => x.Oilseed4Area, model.Oilseed4Area)
                .SetProperty(x => x.Oilseed4ProductionKg, model.Oilseed4ProductionKg)
                .SetProperty(x => x.Oilseed4SalesRupees, model.Oilseed4SalesRupees)
                .SetProperty(x => x.Oilseed4DeficitRupees, model.Oilseed4DeficitRupees)
                // Cash crops
                .SetProperty(x => x.HasCashCrops, model.HasCashCrops)
                .SetProperty(x => x.CashCropSummary, model.CashCropSummary)
                .SetProperty(x => x.CashGingerTurmeric, model.CashGingerTurmeric)
                .SetProperty(x => x.CashOnionGarlic, model.CashOnionGarlic)
                .SetProperty(x => x.CashMushroom, model.CashMushroom)
                .SetProperty(x => x.CashSugarcane, model.CashSugarcane)
                .SetProperty(x => x.CashChili, model.CashChili)
                .SetProperty(x => x.CashPeanut, model.CashPeanut)
                .SetProperty(x => x.Cash1Area, model.Cash1Area)
                .SetProperty(x => x.Cash1ProductionKg, model.Cash1ProductionKg)
                .SetProperty(x => x.Cash1SalesRupees, model.Cash1SalesRupees)
                .SetProperty(x => x.Cash1DeficitRupees, model.Cash1DeficitRupees)
                .SetProperty(x => x.Cash2Area, model.Cash2Area)
                .SetProperty(x => x.Cash2ProductionKg, model.Cash2ProductionKg)
                .SetProperty(x => x.Cash2SalesRupees, model.Cash2SalesRupees)
                .SetProperty(x => x.Cash2DeficitRupees, model.Cash2DeficitRupees)
                .SetProperty(x => x.Cash3Area, model.Cash3Area)
                .SetProperty(x => x.Cash3ProductionKg, model.Cash3ProductionKg)
                .SetProperty(x => x.Cash3SalesRupees, model.Cash3SalesRupees)
                .SetProperty(x => x.Cash3DeficitRupees, model.Cash3DeficitRupees)
                .SetProperty(x => x.Cash4Area, model.Cash4Area)
                .SetProperty(x => x.Cash4ProductionKg, model.Cash4ProductionKg)
                .SetProperty(x => x.Cash4SalesRupees, model.Cash4SalesRupees)
                .SetProperty(x => x.Cash4DeficitRupees, model.Cash4DeficitRupees)
                .SetProperty(x => x.Cash5Area, model.Cash5Area)
                .SetProperty(x => x.Cash5ProductionKg, model.Cash5ProductionKg)
                .SetProperty(x => x.Cash5SalesRupees, model.Cash5SalesRupees)
                .SetProperty(x => x.Cash5DeficitRupees, model.Cash5DeficitRupees)
                .SetProperty(x => x.Cash6Area, model.Cash6Area)
                .SetProperty(x => x.Cash6ProductionKg, model.Cash6ProductionKg)
                .SetProperty(x => x.Cash6SalesRupees, model.Cash6SalesRupees)
                .SetProperty(x => x.Cash6DeficitRupees, model.Cash6DeficitRupees)
                // Fruits
                .SetProperty(x => x.HasFruitCrops, model.HasFruitCrops)
                .SetProperty(x => x.FruitCropSummary, model.FruitCropSummary)
                .SetProperty(x => x.FruitMango, model.FruitMango)
                .SetProperty(x => x.FruitLemon, model.FruitLemon)
                .SetProperty(x => x.FruitPomelo, model.FruitPomelo)
                .SetProperty(x => x.FruitPomegranate, model.FruitPomegranate)
                .SetProperty(x => x.FruitBanana, model.FruitBanana)
                .SetProperty(x => x.FruitAmba, model.FruitAmba)
                .SetProperty(x => x.FruitLichi, model.FruitLichi)
                .SetProperty(x => x.FruitSarifa, model.FruitSarifa)
                .SetProperty(x => x.Fruit1Area, model.Fruit1Area)
                .SetProperty(x => x.Fruit1ProductionKg, model.Fruit1ProductionKg)
                .SetProperty(x => x.Fruit1SalesRupees, model.Fruit1SalesRupees)
                .SetProperty(x => x.Fruit1DeficitRupees, model.Fruit1DeficitRupees)
                .SetProperty(x => x.Fruit2Area, model.Fruit2Area)
                .SetProperty(x => x.Fruit2ProductionKg, model.Fruit2ProductionKg)
                .SetProperty(x => x.Fruit2SalesRupees, model.Fruit2SalesRupees)
                .SetProperty(x => x.Fruit2DeficitRupees, model.Fruit2DeficitRupees)
                .SetProperty(x => x.Fruit3Area, model.Fruit3Area)
                .SetProperty(x => x.Fruit3ProductionKg, model.Fruit3ProductionKg)
                .SetProperty(x => x.Fruit3SalesRupees, model.Fruit3SalesRupees)
                .SetProperty(x => x.Fruit3DeficitRupees, model.Fruit3DeficitRupees)
                .SetProperty(x => x.Fruit4Area, model.Fruit4Area)
                .SetProperty(x => x.Fruit4ProductionKg, model.Fruit4ProductionKg)
                .SetProperty(x => x.Fruit4SalesRupees, model.Fruit4SalesRupees)
                .SetProperty(x => x.Fruit4DeficitRupees, model.Fruit4DeficitRupees)
                .SetProperty(x => x.Fruit5Area, model.Fruit5Area)
                .SetProperty(x => x.Fruit5ProductionKg, model.Fruit5ProductionKg)
                .SetProperty(x => x.Fruit5SalesRupees, model.Fruit5SalesRupees)
                .SetProperty(x => x.Fruit5DeficitRupees, model.Fruit5DeficitRupees)
                .SetProperty(x => x.Fruit6Area, model.Fruit6Area)
                .SetProperty(x => x.Fruit6ProductionKg, model.Fruit6ProductionKg)
                .SetProperty(x => x.Fruit6SalesRupees, model.Fruit6SalesRupees)
                .SetProperty(x => x.Fruit6DeficitRupees, model.Fruit6DeficitRupees)
                .SetProperty(x => x.Fruit7Area, model.Fruit7Area)
                .SetProperty(x => x.Fruit7ProductionKg, model.Fruit7ProductionKg)
                .SetProperty(x => x.Fruit7SalesRupees, model.Fruit7SalesRupees)
                .SetProperty(x => x.Fruit7DeficitRupees, model.Fruit7DeficitRupees)
                .SetProperty(x => x.Fruit8Area, model.Fruit8Area)
                .SetProperty(x => x.Fruit8ProductionKg, model.Fruit8ProductionKg)
                .SetProperty(x => x.Fruit8SalesRupees, model.Fruit8SalesRupees)
                .SetProperty(x => x.Fruit8DeficitRupees, model.Fruit8DeficitRupees)
                // Herbs
                .SetProperty(x => x.HasHerbCrops, model.HasHerbCrops)
                .SetProperty(x => x.HerbCropSummary, model.HerbCropSummary)
                .SetProperty(x => x.HerbHarroBarro, model.HerbHarroBarro)
                .SetProperty(x => x.HerbGurjo, model.HerbGurjo)
                .SetProperty(x => x.HerbTulsi, model.HerbTulsi)
                .SetProperty(x => x.HerbAmla, model.HerbAmla)
                .SetProperty(x => x.HerbSarpagandha, model.HerbSarpagandha)
                .SetProperty(x => x.HerbAjwain, model.HerbAjwain)
                .SetProperty(x => x.HerbSutli, model.HerbSutli)
                .SetProperty(x => x.HerbAloeVera, model.HerbAloeVera)
                .SetProperty(x => x.HerbBojho, model.HerbBojho)
                .SetProperty(x => x.Herb1Area, model.Herb1Area)
                .SetProperty(x => x.Herb1ProductionKg, model.Herb1ProductionKg)
                .SetProperty(x => x.Herb1SalesRupees, model.Herb1SalesRupees)
                .SetProperty(x => x.Herb2Area, model.Herb2Area)
                .SetProperty(x => x.Herb2ProductionKg, model.Herb2ProductionKg)
                .SetProperty(x => x.Herb2SalesRupees, model.Herb2SalesRupees)
                .SetProperty(x => x.Herb3Area, model.Herb3Area)
                .SetProperty(x => x.Herb3ProductionKg, model.Herb3ProductionKg)
                .SetProperty(x => x.Herb3SalesRupees, model.Herb3SalesRupees)
                .SetProperty(x => x.Herb4Area, model.Herb4Area)
                .SetProperty(x => x.Herb4ProductionKg, model.Herb4ProductionKg)
                .SetProperty(x => x.Herb4SalesRupees, model.Herb4SalesRupees)
                .SetProperty(x => x.Herb5Area, model.Herb5Area)
                .SetProperty(x => x.Herb5ProductionKg, model.Herb5ProductionKg)
                .SetProperty(x => x.Herb5SalesRupees, model.Herb5SalesRupees)
                .SetProperty(x => x.Herb6Area, model.Herb6Area)
                .SetProperty(x => x.Herb6ProductionKg, model.Herb6ProductionKg)
                .SetProperty(x => x.Herb6SalesRupees, model.Herb6SalesRupees)
                .SetProperty(x => x.Herb7Area, model.Herb7Area)
                .SetProperty(x => x.Herb7ProductionKg, model.Herb7ProductionKg)
                .SetProperty(x => x.Herb7SalesRupees, model.Herb7SalesRupees)
                .SetProperty(x => x.Herb8Area, model.Herb8Area)
                .SetProperty(x => x.Herb8ProductionKg, model.Herb8ProductionKg)
                .SetProperty(x => x.Herb8SalesRupees, model.Herb8SalesRupees)
                .SetProperty(x => x.Herb9Area, model.Herb9Area)
                .SetProperty(x => x.Herb9ProductionKg, model.Herb9ProductionKg)
                .SetProperty(x => x.Herb9SalesRupees, model.Herb9SalesRupees)
                // Dairy
                .SetProperty(x => x.HasLivestock, model.HasLivestock)
                .SetProperty(x => x.DairySummary, model.DairySummary)
                .SetProperty(x => x.DairyCow, model.DairyCow)
                .SetProperty(x => x.DairyBuffalo, model.DairyBuffalo)
                .SetProperty(x => x.DairyOther, model.DairyOther)
                .SetProperty(x => x.CowCount, model.CowCount)
                .SetProperty(x => x.CowMilkDaily, model.CowMilkDaily)
                .SetProperty(x => x.CowMilkSold, model.CowMilkSold)
                .SetProperty(x => x.CowCurdSold, model.CowCurdSold)
                .SetProperty(x => x.CowAnnualIncome, model.CowAnnualIncome)
                .SetProperty(x => x.BuffaloCount, model.BuffaloCount)
                .SetProperty(x => x.BuffaloMilkDaily, model.BuffaloMilkDaily)
                .SetProperty(x => x.BuffaloMilkSold, model.BuffaloMilkSold)
                .SetProperty(x => x.BuffaloCurdSold, model.BuffaloCurdSold)
                .SetProperty(x => x.BuffaloAnnualIncome, model.BuffaloAnnualIncome)
                .SetProperty(x => x.OtherDairyCount, model.OtherDairyCount)
                .SetProperty(x => x.OtherDairyMilkDaily, model.OtherDairyMilkDaily)
                .SetProperty(x => x.OtherDairyMilkSold, model.OtherDairyMilkSold)
                .SetProperty(x => x.OtherDairyCurdSold, model.OtherDairyCurdSold)
                .SetProperty(x => x.OtherDairyAnnualIncome, model.OtherDairyAnnualIncome)
                .SetProperty(x => x.GheeSold, model.GheeSold)
                .SetProperty(x => x.GheeSold2, model.GheeSold2)
                .SetProperty(x => x.GheeSold3, model.GheeSold3)
                // Meat
                .SetProperty(x => x.MeatSummary, model.MeatSummary)
                .SetProperty(x => x.MeatGoatSheep, model.MeatGoatSheep)
                .SetProperty(x => x.MeatCalf, model.MeatCalf)
                .SetProperty(x => x.MeatPig, model.MeatPig)
                .SetProperty(x => x.MeatOther, model.MeatOther)
                .SetProperty(x => x.GoatCount, model.GoatCount)
                .SetProperty(x => x.GoatAnnualSold, model.GoatAnnualSold)
                .SetProperty(x => x.GoatAnnualIncome, model.GoatAnnualIncome)
                .SetProperty(x => x.CalfCount, model.CalfCount)
                .SetProperty(x => x.CalfAnnualSold, model.CalfAnnualSold)
                .SetProperty(x => x.CalfAnnualIncome, model.CalfAnnualIncome)
                .SetProperty(x => x.PigCount, model.PigCount)
                .SetProperty(x => x.PigAnnualSold, model.PigAnnualSold)
                .SetProperty(x => x.PigAnnualIncome, model.PigAnnualIncome)
                .SetProperty(x => x.OtherMeatCount, model.OtherMeatCount)
                .SetProperty(x => x.OtherMeatAnnualSold, model.OtherMeatAnnualSold)
                .SetProperty(x => x.OtherMeatAnnualIncome, model.OtherMeatAnnualIncome)
                // Poultry
                .SetProperty(x => x.PoultrySummary, model.PoultrySummary)
                .SetProperty(x => x.PoultryChicken, model.PoultryChicken)
                .SetProperty(x => x.PoultryDuck, model.PoultryDuck)
                .SetProperty(x => x.PoultryPigeon, model.PoultryPigeon)
                .SetProperty(x => x.PoultryBattai, model.PoultryBattai)
                .SetProperty(x => x.PoultryCount, model.PoultryCount)
                .SetProperty(x => x.PoultryEggAnnual, model.PoultryEggAnnual)
                .SetProperty(x => x.PoultryMeatSoldKg, model.PoultryMeatSoldKg)
                .SetProperty(x => x.PoultryAnnualIncome, model.PoultryAnnualIncome)
                // Fish
                .SetProperty(x => x.HasFishFarming, model.HasFishFarming)
                .SetProperty(x => x.FishPondCount, model.FishPondCount)
                .SetProperty(x => x.FishPondArea, model.FishPondArea)
                .SetProperty(x => x.FishProductionKg, model.FishProductionKg)
                .SetProperty(x => x.FishAnnualIncome, model.FishAnnualIncome)
                .SetProperty(x => x.FishSubsidy, model.FishSubsidy)
                // Beekeeping
                .SetProperty(x => x.HasBeekeeping, model.HasBeekeeping)
                .SetProperty(x => x.BeeHiveCount, model.BeeHiveCount)
                .SetProperty(x => x.HoneyProductionKg, model.HoneyProductionKg)
                .SetProperty(x => x.HoneyAnnualIncome, model.HoneyAnnualIncome)
                .SetProperty(x => x.BeeSubsidy, model.BeeSubsidy)
                // Income
                .SetProperty(x => x.IncomeVegetables, model.IncomeVegetables)
                .SetProperty(x => x.IncomeLabor, model.IncomeLabor)
                .SetProperty(x => x.IncomeTrade, model.IncomeTrade)
                .SetProperty(x => x.IncomeForeign, model.IncomeForeign)
                .SetProperty(x => x.IncomeJob, model.IncomeJob)
                .SetProperty(x => x.IncomeOther, model.IncomeOther)
                // Expenditure
                .SetProperty(x => x.ExpenseFood, model.ExpenseFood)
                .SetProperty(x => x.ExpenseClothes, model.ExpenseClothes)
                .SetProperty(x => x.ExpenseEducation, model.ExpenseEducation)
                .SetProperty(x => x.ExpenseHealth, model.ExpenseHealth)
                .SetProperty(x => x.ExpenseFestivals, model.ExpenseFestivals)
                .SetProperty(x => x.ExpenseAgriculture, model.ExpenseAgriculture)
                .SetProperty(x => x.ExpenseFuel, model.ExpenseFuel)
                .SetProperty(x => x.ExpenseMobileInternet, model.ExpenseMobileInternet)
                .SetProperty(x => x.FoodSufficiencyMonths, model.FoodSufficiencyMonths)
                // Deficit spending
                .SetProperty(x => x.DeficitSpendingSummary, model.DeficitSpendingSummary)
                .SetProperty(x => x.DeficitJob, model.DeficitJob)
                .SetProperty(x => x.DeficitDailyWage, model.DeficitDailyWage)
                .SetProperty(x => x.DeficitForeignJob, model.DeficitForeignJob)
                .SetProperty(x => x.DeficitTrade, model.DeficitTrade)
                .SetProperty(x => x.DeficitLoan, model.DeficitLoan)
                // Banking
                .SetProperty(x => x.HasBankAccount, model.HasBankAccount)
                .SetProperty(x => x.BankAccountSummary, model.BankAccountSummary)
                .SetProperty(x => x.BankAccountBank, model.BankAccountBank)
                .SetProperty(x => x.BankAccountCoop, model.BankAccountCoop)
                .SetProperty(x => x.BankAccountSavings, model.BankAccountSavings)
                .SetProperty(x => x.BankAccountMoneylender, model.BankAccountMoneylender)
                .SetProperty(x => x.BankAccountOther, model.BankAccountOther)
                .SetProperty(x => x.HasLoan, model.HasLoan)
                .SetProperty(x => x.LoanAmount, model.LoanAmount)
                .SetProperty(x => x.LoanSourceSummary, model.LoanSourceSummary)
                .SetProperty(x => x.LoanFromBank, model.LoanFromBank)
                .SetProperty(x => x.LoanFromCoop, model.LoanFromCoop)
                .SetProperty(x => x.LoanFromMoneylender, model.LoanFromMoneylender)
                .SetProperty(x => x.LoanFromRelatives, model.LoanFromRelatives)
                .SetProperty(x => x.LoanType, model.LoanType)
                .SetProperty(x => x.LoanPurposeSummary, model.LoanPurposeSummary)
                .SetProperty(x => x.LoanPurposeBusiness, model.LoanPurposeBusiness)
                .SetProperty(x => x.LoanPurposeHouse, model.LoanPurposeHouse)
                .SetProperty(x => x.LoanPurposeMedical, model.LoanPurposeMedical)
                .SetProperty(x => x.LoanPurposeEducation, model.LoanPurposeEducation)
                .SetProperty(x => x.LoanPurposeOther, model.LoanPurposeOther)
                .SetProperty(x => x.LoanPurposeOtherDetail, model.LoanPurposeOtherDetail)
            );

        TempData["SuccessMessage"] = "कृषि तथा पशुपालन डाटा सफलतापूर्वक सुरक्षित गरियो।";
        return RedirectToAction(nameof(ManageHouseholdData), new { id = model.Id });
    }
    
    // Configuration map for institutional metadata
    private static readonly Dictionary<string, (Type EntityType, string DisplayName, string Icon, string Color, string PropName)> SubjectAreaConfigs = new()
    {
        { "educational_institution", (typeof(EducationalInstitution), "शैक्षिक संस्था", "bi-school", "#52B788", "EducationalInstitution") },
            { "health_institution", (typeof(HealthInstitution), "स्वास्थ्य संस्था", "bi-hospital", "#E76F51", "HealthInstitution") },
        { "building", (typeof(Building), "भवन", "bi-building", "#B5838D",
            "Building") },
        { "security_body", (typeof(SecurityBody), "सुरक्षा निकाय", "bi-shield-lock",
            "#457B9D", "SecurityBody") },
        { "financial_institution", (typeof(FinancialInstitution), "वित्तीय संस्था",
            "bi-bank", "#E9C46A", "FinancialInstitution") },
        { "hotel_resort", (typeof(HotelResort), "होटल/रिसोर्ट", "bi-building",
            "#D4A373", "HotelResort") },
        { "bridge", (typeof(Bridge), "पुल/पुलेसा", "bi-signpost-2", "#5c6bc0",
            "Bridge") },
        { "industry", (typeof(Industry), "उद्योग", "bi-gear-fill", "#8d6e63",
            "Industry") },
        { "ngo_club_group", (typeof(NgoClubGroup), "गैरसरकारी संस्था/क्लब", "bi-people", "#78909c", "NgoClubGroup") },
        { "land_use", (typeof(LandUse), "भू-उपयोग", "bi-map-fill", "#ab47bc",
            "LandUse") },
        { "natural_resource", (typeof(NaturalResource), "प्राकृतिक श्रोत", "bi-gem",
            "#26a69a", "NaturalResource") },
        { "cultural_heritage", (typeof(CulturalHeritage), "सांस्कृतिक सम्पदा", "bi-bank2", "#ec407a", "CulturalHeritage") },
        { "trade_business", (typeof(TradeBusiness), "व्यापार/व्यवसाय", "bi-shop",
            "#d4e157", "TradeBusiness") },
        { "forest", (typeof(Forest), "वन र वनस्पति", "bi-tree-fill", "#66bb6a",
            "Forest") },
        { "road", (typeof(Road), "सडक पूर्वाधार", "bi-road", "#ff7043", "Road") },
        { "ward_statistics", (typeof(WardStatistics), "वडा सांख्यिकी", "bi-barchart", "#26c6da", "WardStatistics") }
    };
    
    [HttpGet]
    public async Task<IActionResult> Institutions(string subjectArea = "educational_institution", int page = 1, string? search = null, int? wardNo = null)
    {
        if (!SubjectAreaConfigs.ContainsKey(subjectArea))
            return NotFound();

        var config = SubjectAreaConfigs[subjectArea];
        ViewBag.SubjectArea = subjectArea;
        ViewBag.DisplayName = config.DisplayName;
        ViewBag.Search = search;
        ViewBag.WardNo = wardNo;
        ViewBag.ConfigList = SubjectAreaConfigs
            .Select(kvp => new InstitutionCategoryItem
            {
                Key         = kvp.Key,
                DisplayName = kvp.Value.DisplayName,
                Icon        = kvp.Value.Icon
            }).ToList();

        const int pageSize = 20;

        // ── FIXED: query the child entity IDs first, then join to survey_entry ──
        // This works regardless of whether survey_entry.subject_area is populated.
        List<long> surveyEntryIds;
        if (ChildFetchers.TryGetValue(subjectArea, out _))
        {
            surveyEntryIds = await GetChildSurveyEntryIdsAsync(subjectArea, wardNo);
        }
        else
        {
            surveyEntryIds = new List<long>();
        }

        var query = _db.SurveyEntries
            .Where(s => surveyEntryIds.Contains(s.Id) && s.Status != "deleted")
            .OrderByDescending(s => s.Id);

        // Optional: apply text search on enumerator name or submitted_by
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = (IOrderedQueryable<SurveyEntry>)query.Where(s =>
                (s.EnumeratorName != null && s.EnumeratorName.Contains(search)) ||
                (s.SubmittedBy != null && s.SubmittedBy.Contains(search)));
        }

        int totalCount = await query.CountAsync();
        var rawData = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new {
                s.Id, s.WardNo, s.EnumeratorName, s.SubmittedBy, s.SubmissionTime
            })
            .ToListAsync();

        // Resolve display names from child entity
        var listData = new List<InstitutionListItem>();
        foreach (var item in rawData)
        {
            string detailName = $"#{subjectArea} #{item.Id}";
            if (ChildFetchers.TryGetValue(subjectArea, out var fetcher))
            {
                var child = await fetcher(_db, item.Id);
                if (child != null)
                {
                    var nameProp = child.GetType().GetProperty("Name")
                        ?? child.GetType().GetProperty("HotelName")
                        ?? child.GetType().GetProperty("ForestName")
                        ?? child.GetType().GetProperty("OrgName")
                        ?? child.GetType().GetProperty("RoadName")
                        ?? child.GetType().GetProperty("BridgeType")
                        ?? child.GetType().GetProperty("IndustryType")
                        ?? child.GetType().GetProperty("BusinessName")
                        ?? child.GetType().GetProperty("LandType")
                        ?? child.GetType().GetProperty("ResourceType")
                        ?? child.GetType().GetProperty("HeritageType")
                        ?? child.GetType().GetProperty("BodyType")
                        ?? child.GetType().GetProperty("BuildingType");
                    if (nameProp != null)
                        detailName = nameProp.GetValue(child)?.ToString() ?? detailName;
                }
            }
            string submissionDisplay = "-";
            if (DateTime.TryParse(item.SubmissionTime, out var parsedDt))
                submissionDisplay = parsedDt.ToString("yyyy-MM-dd HH:mm");
            else if (!string.IsNullOrEmpty(item.SubmissionTime))
                submissionDisplay = item.SubmissionTime;

            listData.Add(new InstitutionListItem
            {
                Id             = item.Id,
                WardNo         = item.WardNo,
                EnumeratorName = item.EnumeratorName,
                SubmittedBy    = item.SubmittedBy,
                Name           = detailName,
                SubmissionTime = submissionDisplay
            });
        }

        ViewBag.TotalPages   = (int)Math.Ceiling((double)totalCount / pageSize);
        ViewBag.CurrentPage  = page;
        ViewBag.TotalItems   = totalCount;

        return View("Institutions/List", listData);
    }
    
    [HttpGet]
    public async Task<IActionResult> EditInstitution(string subjectArea, long
        id)
    {
        if (!SubjectAreaConfigs.ContainsKey(subjectArea)) return NotFound();
        var config = SubjectAreaConfigs[subjectArea];

        // Safe projection — never loads TIMESTAMPTZ columns (submission_time, survey_start, etc.)
        // which cause InvalidCastException when the DB still has them as TIMESTAMPTZ type.
        var parentData = await _db.SurveyEntries
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id, s.WardNo, s.EnumeratorName, s.PhoneNumber, s.DeviceId,
                s.SubmittedBy, s.Status, s.ValidationStatus, s.SubjectArea,
                s.Municipality, s.Notes, s.Tags, s.OdkId, s.OdkIndex, s.Username
            })
            .FirstOrDefaultAsync();

        if (parentData == null) return NotFound();

        if (IsWardAdmin && UserWard.HasValue && parentData.WardNo != UserWard.Value)
            return Forbid();

        // Reconstruct a minimal SurveyEntry to pass to the view (no timestamp fields needed)
        var parent = new SurveyEntry
        {
            Id             = parentData.Id,
            WardNo         = parentData.WardNo,
            EnumeratorName = parentData.EnumeratorName,
            PhoneNumber    = parentData.PhoneNumber,
            DeviceId       = parentData.DeviceId,
            SubmittedBy    = parentData.SubmittedBy,
            Status         = parentData.Status,
            ValidationStatus = parentData.ValidationStatus,
            SubjectArea    = parentData.SubjectArea,
            Municipality   = parentData.Municipality,
            Notes          = parentData.Notes,
            Tags           = parentData.Tags,
            OdkId          = parentData.OdkId,
            OdkIndex       = parentData.OdkIndex,
            Username       = parentData.Username
        };
        
        // Fetch child via safe typed fetcher (replaces broken IQueryable<object> cast)
        object? child = null;
        if (ChildFetchers.TryGetValue(subjectArea, out var fetcher))
            child = await fetcher(_db, id);
        if (child == null)
        {
            // Auto-create child if it doesn't exist to prevent crashes
            child = Activator.CreateInstance(config.EntityType)!;
            config.EntityType.GetProperty("SurveyEntryId")?.SetValue(child, id);
            _db.Add(child);
            await _db.SaveChangesAsync();
        }
        var viewModel = new DynamicFormViewModel
        {
            Id = id,
            SubjectArea = subjectArea,
            DisplayName = config.DisplayName,
            SurveyEntry = parent,
            ChildEntity = child
        };
        return View("~/Views/Admin/Institutions/Form.cshtml", viewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditInstitution(string subjectArea, long
        id, IFormCollection form)
    {
        if (!SubjectAreaConfigs.ContainsKey(subjectArea)) return NotFound();
        var config = SubjectAreaConfigs[subjectArea];

        // Safe projection — avoids loading TIMESTAMPTZ columns that cause InvalidCastException.
        var parentData = await _db.SurveyEntries
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new { s.Id, s.WardNo })
            .FirstOrDefaultAsync();

        if (parentData == null) return NotFound();

        if (IsWardAdmin && UserWard.HasValue && parentData.WardNo != UserWard.Value)
            return Forbid();
        
        // 1. Update Survey Entry base values via ExecuteUpdateAsync (safe — no full entity load, no TIMESTAMPTZ risk)
        var newWardNo = int.TryParse(form["SurveyEntry.WardNo"], out var ward) ? ward : parentData.WardNo;
        await _db.SurveyEntries
            .Where(s => s.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.WardNo,         newWardNo)
                .SetProperty(x => x.EnumeratorName, form["SurveyEntry.EnumeratorName"].ToString())
                .SetProperty(x => x.PhoneNumber,    form["SurveyEntry.PhoneNumber"].ToString())
                .SetProperty(x => x.DeviceId,       form["SurveyEntry.DeviceId"].ToString())
                .SetProperty(x => x.Notes,          form["SurveyEntry.Notes"].ToString()));

        // 2. Query and Bind child properties dynamically via safe typed fetcher
        object? child = null;
        if (ChildFetchers.TryGetValue(subjectArea, out var fetcher))
            child = await fetcher(_db, id);
        if (child != null)
        {
            foreach (var prop in config.EntityType.GetProperties())
            {
                if (prop.Name == "Id" || prop.Name == "SurveyEntryId" ||
                    prop.Name == "SurveyEntry")
                    continue;
                string formKey = "ChildEntity." + prop.Name;
                if (form.TryGetValue(formKey, out var val))
                {
                    try
                    {
                        var t = Nullable.GetUnderlyingType(prop.PropertyType) ??
                                prop.PropertyType;
                        object? convertedValue = null;
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (t == typeof(double))
                                convertedValue = double.Parse(val);
                            else if (t == typeof(int))
                                convertedValue = int.Parse(val);
                            else if (t == typeof(short))
                                convertedValue = short.Parse(val);
                            else if (t == typeof(long))
                                convertedValue = long.Parse(val);
                            else
                                convertedValue =
                                    Convert.ChangeType(val.ToString(), t);
                        }
                        prop.SetValue(child, convertedValue);
                    }
                    catch { /* Handle parsing failures gracefully */ }
                }
            }
        }
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "संस्थागत विवरण सफलतापूर्वक परिमार्जन गरियो।";
        return RedirectToAction(nameof(Institutions), new { subjectArea });
    }
    
    [HttpGet]
    public IActionResult CreateInstitution(string subjectArea)
    {
        if (!SubjectAreaConfigs.ContainsKey(subjectArea)) return NotFound();
        var config = SubjectAreaConfigs[subjectArea];
        var parent = new SurveyEntry { SubjectArea = subjectArea, Municipality =
            "वृन्दावन नगरपालिका" };
        var child = Activator.CreateInstance(config.EntityType)!;
        var viewModel = new DynamicFormViewModel
        {
            Id = 0,
            SubjectArea = subjectArea,
            DisplayName = config.DisplayName,
            SurveyEntry = parent,
            ChildEntity = child
        };
        return View("Institutions/Form", viewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateInstitution(string subjectArea,
        IFormCollection form)
    {
        if (!SubjectAreaConfigs.ContainsKey(subjectArea)) return NotFound();
        
        if (IsWardAdmin && UserWard.HasValue)
        {
            if (!int.TryParse(form["SurveyEntry.WardNo"], out var formWard) || formWard != UserWard.Value)
            {
                TempData["ErrorMessage"] =
                    $"तपाईं केवल वडा नं. {UserWard.Value} को संस्थागत डाटा थप्न सक्नुहुन्छ।";
                return RedirectToAction(nameof(Institutions), new { subjectArea });
            }
        }
        
        var config = SubjectAreaConfigs[subjectArea];

        var parent = new SurveyEntry
        {
            SubjectArea = subjectArea,
            Municipality = "वृन्दावन नगरपालिका",
            WardNo = int.TryParse(form["SurveyEntry.WardNo"], out var ward) ?
                ward : 1,
            EnumeratorName = form["SurveyEntry.EnumeratorName"],
            PhoneNumber = form["SurveyEntry.PhoneNumber"],
            DeviceId = form["SurveyEntry.DeviceId"],
            Notes = form["SurveyEntry.Notes"],
            Status = "submitted",
            ValidationStatus = "approved",
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            SubmissionTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
        _db.SurveyEntries.Add(parent);
        await _db.SaveChangesAsync(); // Generates ID
        // 2. Create Child Entity
        var child = Activator.CreateInstance(config.EntityType)!;
        config.EntityType.GetProperty("SurveyEntryId")?.SetValue(child,
            parent.Id);
        foreach (var prop in config.EntityType.GetProperties())
        {
            if (prop.Name == "Id" || prop.Name == "SurveyEntryId" || prop.Name
                == "SurveyEntry")
                continue;
            string formKey = "ChildEntity." + prop.Name;
            if (form.TryGetValue(formKey, out var val))
            {
                try
                {
                    var t = Nullable.GetUnderlyingType(prop.PropertyType) ??
                            prop.PropertyType;
                    object? convertedValue = null;
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (t == typeof(double))
                            convertedValue = double.Parse(val);
                        else if (t == typeof(int))
                            convertedValue = int.Parse(val);
                        else if (t == typeof(short))
                            convertedValue = short.Parse(val);
                        else if (t == typeof(long))
                            convertedValue = long.Parse(val);
                        else
                            convertedValue = Convert.ChangeType(val.ToString(), t);
                    }
                    prop.SetValue(child, convertedValue);
                }
                catch { }
            }
        }
        // Use _db.Add(child) — DO NOT use reflection to find DbSet.Add(); it is unreliable
        _db.Add(child);
        
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "नयाँ संस्थागत प्रविष्टि सफलतापूर्वक थपियो।";
        
        return RedirectToAction(nameof(Institutions), new { subjectArea });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]  
    public async Task<IActionResult> DeleteInstitution(long id, string subjectArea)
    {
        // Soft delete — set status to "deleted", do NOT remove from DB
        await _db.SurveyEntries
            .Where(s => s.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "deleted"));

        TempData["SuccessMessage"] = "प्रविष्टि ट्रेसमा सारियो।";
        return RedirectToAction(nameof(Institutions), new { subjectArea });
    }
    
    [HttpGet]
    public async Task<IActionResult> InstitutionsTrash(string subjectArea = "educational_institution", int page = 1)
    {
        if (!SubjectAreaConfigs.ContainsKey(subjectArea))
            return NotFound();

        var config = SubjectAreaConfigs[subjectArea];
        ViewBag.SubjectArea  = subjectArea;
        ViewBag.DisplayName  = config.DisplayName;
        ViewBag.ConfigList   = SubjectAreaConfigs
            .Select(kvp => new InstitutionCategoryItem
            {
                Key         = kvp.Key,
                DisplayName = kvp.Value.DisplayName,
                Icon        = kvp.Value.Icon
            }).ToList();

        const int pageSize = 20;

        // Get IDs of deleted survey_entries that have a child in this subjectArea
        var allIds   = await GetChildSurveyEntryIdsAsync(subjectArea, wardNo: null);
        var trashIds = await _db.SurveyEntries
            .Where(s => allIds.Contains(s.Id) && s.Status == "deleted")
            .Select(s => s.Id)
            .ToListAsync();

        var rawData = await _db.SurveyEntries
            .Where(s => trashIds.Contains(s.Id))
            .OrderByDescending(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new { s.Id, s.WardNo, s.EnumeratorName, s.SubmittedBy, s.SubmissionTime })
            .ToListAsync();

        var listData = new List<InstitutionListItem>();
        foreach (var item in rawData)
        {
            string detailName = $"#{subjectArea} #{item.Id}";
            if (ChildFetchers.TryGetValue(subjectArea, out var fetcher))
            {
                var child = await fetcher(_db, item.Id);
                if (child != null)
                {
                    var nameProp = child.GetType().GetProperty("Name")
                        ?? child.GetType().GetProperty("HotelName")
                        ?? child.GetType().GetProperty("ForestName")
                        ?? child.GetType().GetProperty("OrgName")
                        ?? child.GetType().GetProperty("RoadName")
                        ?? child.GetType().GetProperty("BridgeType")
                        ?? child.GetType().GetProperty("IndustryType")
                        ?? child.GetType().GetProperty("BusinessName")
                        ?? child.GetType().GetProperty("LandType")
                        ?? child.GetType().GetProperty("ResourceType")
                        ?? child.GetType().GetProperty("HeritageType")
                        ?? child.GetType().GetProperty("BodyType")
                        ?? child.GetType().GetProperty("BuildingType");
                    if (nameProp != null)
                        detailName = nameProp.GetValue(child)?.ToString() ?? detailName;
                }
            }
            listData.Add(new InstitutionListItem
            {
                Id             = item.Id,
                WardNo         = item.WardNo,
                EnumeratorName = item.EnumeratorName,
                SubmittedBy    = item.SubmittedBy,
                Name           = detailName,
                SubmissionTime = item.SubmissionTime ?? "-"
            });
        }

        ViewBag.TotalPages  = (int)Math.Ceiling((double)trashIds.Count / pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.TotalItems  = trashIds.Count;

        return View("Institutions/Trash", listData);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreInstitution(long id, string subjectArea)
    {
        await _db.SurveyEntries
            .Where(s => s.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "submitted"));

        TempData["SuccessMessage"] = "प्रविष्टि सफलतापूर्वक पुनर्स्थापित गरियो।";
        return RedirectToAction(nameof(InstitutionsTrash), new { subjectArea });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]  
    public async Task<IActionResult> HardDeleteInstitution(long id, string subjectArea)
    {
        // Use ExecuteDeleteAsync to avoid loading the full entity (no TIMESTAMPTZ cast risk).
        // Delete child first to respect FK constraints, then delete parent.
        var exists = await _db.SurveyEntries.AnyAsync(s => s.Id == id);
        if (exists)
        {
            // Delete child first using the passed subjectArea (NOT parent.SubjectArea which may be null)
            if (ChildFetchers.TryGetValue(subjectArea, out var fetcher))
            {
                var child = await fetcher(_db, id);
                if (child != null)
                    _db.Remove(child);
                await _db.SaveChangesAsync();
            }
            // Delete parent survey entry without loading it (avoids TIMESTAMPTZ cast)
            await _db.SurveyEntries
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();
        }
        TempData["SuccessMessage"] = "प्रविष्टि स्थायी रूपमा मेटाइयो।";
        return RedirectToAction(nameof(InstitutionsTrash), new { subjectArea });
    }
    
    // ── ChildFetchers: Add as private static readonly field
    // This safely replaces all IQueryable<object> reflection patterns.
    private static readonly Dictionary<string, Func<AppDbContext, long,
        Task<object?>>> ChildFetchers = new()
    {
        { "educational_institution", async (db, id) => await
            db.EducationalInstitutions.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "health_institution", async (db, id) => await
            db.HealthInstitutions.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "building", async (db, id) => await
            db.Buildings.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "security_body", async (db, id) => await
            db.SecurityBodies.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "financial_institution", async (db, id) => await
            db.FinancialInstitutions.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "hotel_resort", async (db, id) => await
            db.HotelResorts.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "bridge", async (db, id) => await
            db.Bridges.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "industry", async (db, id) => await 
            db.Industries.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "ngo_club_group", async (db, id) => await 
            db.NgoClubGroups.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "land_use", async (db, id) => await
            db.LandUses.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "natural_resource", async (db, id) => await
            db.NaturalResources.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "cultural_heritage", async (db, id) => await
            db.CulturalHeritages.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "trade_business", async (db, id) => await
            db.TradeBusinesses.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "forest", async (db, id) => await
            db.Forests.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "road", async (db, id) => await
            db.Roads.FirstOrDefaultAsync(e => e.SurveyEntryId == id) },
        { "ward_statistics", async (db, id) => await
            db.WardStatistics.FirstOrDefaultAsync(e => e.SurveyEntryId == id) }
    };
    
    /// <summary>
    /// Returns the SurveyEntryId values that have a child row in the given entity table.
    /// This fixes the empty list bug — child tables have data even when survey_entry.subject_area is null.
    /// Optionally filters by ward number.
    /// </summary>
    private async Task<List<long>> GetChildSurveyEntryIdsAsync(string subjectArea, int? wardNo = null)
    {
        // We query each DbSet directly via a typed switch for type safety.
        // EF Core cannot handle IQueryable<object>, so we must enumerate cases.
        IQueryable<long> idQuery = subjectArea switch
        {
            "educational_institution" => _db.EducationalInstitutions
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "health_institution" => _db.HealthInstitutions
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "building" => _db.Buildings
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "security_body" => _db.SecurityBodies
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "financial_institution" => _db.FinancialInstitutions
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "hotel_resort" => _db.HotelResorts
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "bridge" => _db.Bridges
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "industry" => _db.Industries
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "ngo_club_group" => _db.NgoClubGroups
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "land_use" => _db.LandUses
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "natural_resource" => _db.NaturalResources
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "cultural_heritage" => _db.CulturalHeritages
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "trade_business" => _db.TradeBusinesses
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "forest" => _db.Forests
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "road" => _db.Roads
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            "ward_statistics" => _db.WardStatistics
                .Join(_db.SurveyEntries, e => e.SurveyEntryId, s => s.Id,
                      (e, s) => new { e.SurveyEntryId, s.WardNo })
                .Where(x => wardNo == null || x.WardNo == wardNo)
                .Select(x => x.SurveyEntryId),

            _ => Enumerable.Empty<long>().AsQueryable()
        };

        return await idQuery.Distinct().ToListAsync();
    }
    
    // ═══════════════════════════════════════════════════
    // HOUSEHOLD CHILD TABLES CRUD (Family, House, Deaths)
    // ═══════════════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> ManageSubTables(long id, string tab = "family")
    {
        var hh = await _db.Households
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new { h.Id, h.RespondentName, h.WardNo,
                h.SettlementName })
            .FirstOrDefaultAsync();
        if (hh == null) return NotFound();
        
        ViewBag.HouseholdId = hh.Id;
        ViewBag.Info = $"{hh.RespondentName} - वडा {hh.WardNo}({hh.SettlementName})";
        ViewBag.ActiveTab = tab;
        var familyMembers = await _db.FamilyMembers.Where(f => f.HouseholdId ==
                                                               id).ToListAsync();
        var houseDetails = await _db.HouseDetails.Where(h => h.HouseholdId ==
                                                             id).ToListAsync();
        var deathRecords = await _db.HhDeathRecords.Where(d => d.HouseholdId ==
                                                               id).ToListAsync();
        var model = new HouseholdSubTablesViewModel
        {
            HouseholdId = id,
            FamilyMembers = familyMembers,
            HouseDetails = houseDetails,
            DeathRecords = deathRecords
        };
        return View("HouseHolds/SubTables", model);
    }
    // --- Family Member POST Actions ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFamilyMember(long householdId,
        FamilyMember member)
    {
        member.HouseholdId = householdId;

        // Two-way sync: DobBs ↔ Age
        if (!string.IsNullOrWhiteSpace(member.DobBs))
        {
            // If DOB supplied, always recalculate age from it
            member.Age = NepaliDateHelper.CalculateAgeBs(member.DobBs);
        }
        else if (member.Age.HasValue && member.Age.Value > 0)
        {
            // If only age supplied, derive an approximate BS DOB
            member.DobBs = NepaliDateHelper.ApproxDobBsFromAge((int)member.Age.Value);
        }

        _db.FamilyMembers.Add(member);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "पारिवारिक सदस्य विवरण सफलतापूर्वक थपियो।";
        return RedirectToAction(nameof(ManageSubTables), new { id = householdId,
            tab = "family" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFamilyMember(FamilyMember model)
    {
        var member = await _db.FamilyMembers.FindAsync(model.Id);
        if (member == null) return NotFound();
        // Bind modified values
        member.FullName = model.FullName;
        member.Surname = model.Surname;
        member.Gender = model.Gender;
        member.RelationToHead = model.RelationToHead;
        member.EducationLevel = model.EducationLevel;
        member.Occupation = model.Occupation;
        member.SocialSecurityStatus = model.SocialSecurityStatus;
        member.DobBs = model.DobBs;

        // Two-way sync: DobBs ↔ Age
        if (!string.IsNullOrWhiteSpace(member.DobBs))
        {
            // Recalculate age from DOB (always current)
            member.Age = NepaliDateHelper.CalculateAgeBs(member.DobBs);
        }
        else
        {
            member.Age = model.Age;
            if (member.Age.HasValue && member.Age.Value > 0)
                member.DobBs = NepaliDateHelper.ApproxDobBsFromAge((int)member.Age.Value);
        }

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "सदस्य विवरण अद्यावधिक भयो।";
        return RedirectToAction(nameof(ManageSubTables), new
        {
            id = member.HouseholdId,
            tab = "family"
        });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditHouseDetail(HouseDetail model)
    {
        var detail = await _db.HouseDetails.FindAsync(model.Id);
        if (detail == null) return NotFound();
        
        // Update fields
        detail.OwnershipType = model.OwnershipType;
        detail.TotalRooms = model.TotalRooms;
        detail.RoofType = model.RoofType;
        
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "घरको विवरण सफलतापूर्वक अद्यावधिक भयो।";
        return RedirectToAction(nameof(ManageSubTables), new { id = detail.HouseholdId, tab = "house" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDeathRecord(HhDeathRecord model)
    {
        var record = await _db.HhDeathRecords.FindAsync(model.Id);
        if (record == null) return NotFound();
        
        // Update fields
        record.DeceasedName = model.DeceasedName;
        record.DeceasedGender = model.DeceasedGender;
        record.DeceasedAgeDetail = model.DeceasedAgeDetail;
        record.CauseSummary = model.CauseSummary;
        
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "मृत्यु विवरण सफलतापूर्वक अद्यावधिक भयो।";
        return RedirectToAction(nameof(ManageSubTables), new { id = record.HouseholdId, tab = "death" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]  
    public async Task<IActionResult> DeleteFamilyMember(long id)
    {
        var member = await _db.FamilyMembers.FindAsync(id);
        if (member == null) return NotFound();
        long hhId = member.HouseholdId ?? 0;
        _db.FamilyMembers.Remove(member);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "पारिवारिक सदस्य हटाइयो।";
        return RedirectToAction(nameof(ManageSubTables), new { id = hhId, tab =
            "family" });
    }
    // --- House Detail POST Actions ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddHouseDetail(long householdId,
        HouseDetail detail)
    {
        detail.HouseholdId = householdId;
        _db.HouseDetails.Add(detail);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "घरको विवरण सफलतापूर्वक थपियो।";
        return RedirectToAction(nameof(ManageSubTables), new { id = householdId,
            tab = "house" });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]  
    public async Task<IActionResult> DeleteHouseDetail(long id)
    {
        var detail = await _db.HouseDetails.FindAsync(id);
        if (detail == null) return NotFound();
        long hhId = detail.HouseholdId ?? 0;
        _db.HouseDetails.Remove(detail);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "घरको विवरण हटाइयो।";
        return RedirectToAction(nameof(ManageSubTables), new { id = hhId, tab =
            "house" });
    }
    // --- Death Record POST Actions ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDeathRecord(long householdId,
        HhDeathRecord record)
    {
        record.HouseholdId = householdId;
        _db.HhDeathRecords.Add(record);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "मृत्यु विवरण सफलतापूर्वक थपियो।";
        return RedirectToAction(nameof(ManageSubTables), new { id = householdId,
            tab = "death" });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]  
    public async Task<IActionResult> DeleteDeathRecord(long id)
    {
        var record = await _db.HhDeathRecords.FindAsync(id);
        if (record == null) return NotFound();
        long hhId = record.HouseholdId ?? 0;
        _db.HhDeathRecords.Remove(record);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "मृत्यु विवरण हटाइयो।";
        return RedirectToAction(nameof(ManageSubTables), new { id = hhId, tab =
            "death" });
    }
    
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Users()
    {
        ViewData["Title"] = "प्रशासक व्यवस्थापन";
        var users = await _db.AppUsers
            .OrderBy(u => u.Role)
            .ThenByDescending(u => u.Id)
            .ToListAsync();
        return View(users);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult CreateUser()
    {
        ViewData["Title"] = "नयाँ प्रशासक थप्नुहोस्";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateUser(
        string username,
        string email,
        string password,
        string role,
        int? wardNo,
        bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email)    ||
            string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "सबै अनिवार्य क्षेत्रहरू भर्नुहोस्।");
            return View();
        }

        var exists = await _db.AppUsers.AnyAsync(u => u.Username == username);
        if (exists)
        {
            ModelState.AddModelError("", "यो युजरनेम पहिले नै अवस्थित छ।");
            return View();
        }

        // ward_admin must have a ward assigned
        if (role == "ward_admin" && !wardNo.HasValue)
        {
            ModelState.AddModelError("wardNo", "वडा प्रशासकका लागि वडा नम्बर अनिवार्य छ।");
            return View();
        }

        var user = new AppUser
        {
            Username     = username,
            Email        = email,
            PasswordHash = PasswordHasher.Hash(password),
            Role         = role,
            WardNo       = role == "ward_admin" ? wardNo : null,
            IsActive     = isActive,
            CreatedAt    = DateTime.UtcNow
        };

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{username}' ({(role == "ward_admin" ? $"वडा {wardNo} प्रशासक" : "मुख्य प्रशासक")}) सफलतापूर्वक सिर्जना गरियो।";
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> EditUser(long id)
    {
        ViewData["Title"] = "प्रशासक सम्पादन";
        var user = await _db.AppUsers.FindAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> EditUser(
        long   id,
        string email,
        string? password,
        string role,
        int?   wardNo,
        bool   isActive = false)  // checkbox: unchecked = false
    {
        var user = await _db.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        user.Email    = email;
        user.Role     = role;
        user.WardNo   = role == "ward_admin" ? wardNo : null;
        user.IsActive = isActive;

        if (!string.IsNullOrWhiteSpace(password))
            user.PasswordHash = PasswordHasher.Hash(password);

        _db.Entry(user).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{user.Username}' सफलतापूर्वक अद्यावधिक गरियो।";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var user = await _db.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        // Cannot delete your own account
        if (string.Equals(user.Username, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "तपाईंले आफ्नै लगइन खाता मेटाउन सक्नुहुन्न।";
            return RedirectToAction(nameof(Users));
        }

        _db.AppUsers.Remove(user);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{user.Username}' सफलतापूर्वक मेटाइयो।";
        return RedirectToAction(nameof(Users));
    }
}

// ── Helper models — add as separate top-level classes in AdminController.cs ──
public class InstitutionListItem
{
    public long Id { get; set; }
    public int? WardNo { get; set; }
    public string? EnumeratorName { get; set; }
    public string? SubmittedBy { get; set; }
    public string Name { get; set; } = "";
    public string SubmissionTime { get; set; } = "";
}
public class DynamicFormViewModel
{
    public long Id { get; set; }
    public string SubjectArea { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public SurveyEntry SurveyEntry { get; set; } = null!;
    public object ChildEntity { get; set; } = null!;
}
// ── Sidebar navigation item — safe ViewBag POCO (replaces fragile tuple cast)
public class InstitutionCategoryItem
{
    public string Key { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Icon { get; set; } = "";
}