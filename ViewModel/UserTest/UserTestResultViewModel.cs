using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.UserTest;

public class UserTestResultViewModel
{
    public TestResult Result { get; set; } = null!;
    public List<object> History { get; set; } = new();
}
