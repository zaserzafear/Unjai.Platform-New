namespace Unjai.Platform.Mvc.CustomerUser.Models.Home;

public sealed class MeViewModel
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
