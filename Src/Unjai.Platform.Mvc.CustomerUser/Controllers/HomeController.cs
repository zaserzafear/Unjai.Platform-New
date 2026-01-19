using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Unjai.Platform.Infrastructure.RateLimiting.Configurations;
using Unjai.Platform.Infrastructure.RateLimiting.Filters;
using Unjai.Platform.Mvc.CustomerUser.Models;
using Unjai.Platform.Mvc.CustomerUser.Models.Home;

namespace Unjai.Platform.Mvc.CustomerUser.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [RequireRateLimiting(RateLimitPolicyKeys.GetUser)]
    public IActionResult Me()
    {
        var model = new MeViewModel
        {
            FullName = User.FindFirst("name")?.Value ?? "-",
            Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "-",
            PhoneNumber = User.FindFirst(ClaimTypes.MobilePhone)?.Value ?? "-",
            CreatedAt = DateTime.Now
        };

        return View(model);
    }
}
