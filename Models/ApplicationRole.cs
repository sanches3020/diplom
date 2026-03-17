using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models;

public class ApplicationRole : IdentityRole
{
    [StringLength(200)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName, string? description = null)
        : base(roleName)
    {
        Description = description;
    }
}
