using Microsoft.AspNetCore.Identity;

namespace EnterpriseCMS.Core.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public Guid TenantId { get; set; }
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public string Permissions { get; set; } = "[]";
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}
