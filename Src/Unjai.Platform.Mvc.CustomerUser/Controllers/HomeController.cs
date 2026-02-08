using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Unjai.Platform.Contracts.CustomerUsers.Dtos;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Filters;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Mvc.CustomerUser.Configurations;
using Unjai.Platform.Mvc.CustomerUser.Models;
using Unjai.Platform.Mvc.CustomerUser.Models.Home;

namespace Unjai.Platform.Mvc.CustomerUser.Controllers;

public class HomeController(IHttpClientFactory httpClientFactory) : Controller
{
    [EnforceRateLimit(RateLimitPolicyKeys.Default)]
    public IActionResult Index()
    {
        return View();
    }

    [Route("privacy")]
    [EnforceRateLimit(RateLimitPolicyKeys.Default)]
    public IActionResult Privacy()
    {
        return View();
    }

    [Route("error")]
    [EnforceRateLimit(RateLimitPolicyKeys.Default)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("me")]
    [EnforceRateLimit(RateLimitPolicyKeys.GetUser)]
    public async Task<IActionResult> MeAsync(CancellationToken ct)
    {
        var httpClient = httpClientFactory.CreateClient(
            HttpClientNames.InternalApi);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "api/v1/customer-users/123e4567-e89b-12d3-a456-426614174000");

        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<GetCustomerUserReponseDto>>(
        cancellationToken: ct);

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
