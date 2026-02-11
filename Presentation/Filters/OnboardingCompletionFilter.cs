using System.Security.Claims;
using Application.Services.Onboarding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters;

public class OnboardingCompletionFilter : IAsyncActionFilter
{
    private readonly IOnboardingService _onboardingService;

    public OnboardingCompletionFilter(IOnboardingService onboardingService)
    {
        _onboardingService = onboardingService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            await next();
            return;
        }

        if (user.IsInRole("Admin"))
        {
            await next();
            return;
        }

        var controller = context.RouteData.Values["controller"]?.ToString() ?? string.Empty;
        if (string.Equals(controller, "Account", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(controller, "Onboarding", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            await next();
            return;
        }

        var isComplete = await _onboardingService.IsOnboardingCompleteAsync(userId);
        if (!isComplete)
        {
            context.Result = new RedirectToActionResult("Index", "Onboarding", null);
            return;
        }

        await next();
    }
}
