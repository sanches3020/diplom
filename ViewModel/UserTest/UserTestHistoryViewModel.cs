using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.UserTest;

public class UserTestHistoryViewModel
{
    public Test Test { get; set; } = null!;
    public List<TestResult> Results { get; set; } = new();
}
