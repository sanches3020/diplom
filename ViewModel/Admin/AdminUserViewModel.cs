using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Admin;

public class AdminUserViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public IList<string> Roles { get; set; } = new List<string>();
}