using System.ComponentModel.DataAnnotations.Schema;

namespace VrindavanProfile.Web.Data.DbEntities;
 
[Table("app_user")]
public class AppUser
{
    [Column("id")]
    public long Id { get; set; }

    [Column("username")] public string Username { get; set; } = null!;

    [Column("password_hash")] public string PasswordHash { get; set; } = null!;

    [Column("email")] public string Email { get; set; } = null!;

    [Column("role")] public string Role { get; set; } = "admin"; // "admin" or "ward_admin"
    
    [Column("ward_no")] public int? WardNo { get; set; }
    
    [Column("is_active")] public bool IsActive { get; set; } = true;
    
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}