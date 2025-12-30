using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Unjai.Platform.Infrastructure.RateLimiting;
using Unjai.Platform.Mvc.CustomerUser.Models;

namespace Unjai.Platform.Mvc.CustomerUser.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [RequireRateLimiting("get-user")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
