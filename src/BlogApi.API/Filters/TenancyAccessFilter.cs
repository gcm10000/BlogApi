using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BlogApi.Application.Constants;

namespace BlogApi.API.Filters;

public class TenancyAccessFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        var routeTenancyId = context.RouteData.Values["tenancyId"]?.ToString();
        var userTenancyId = user.FindFirst(CustomClaimTypes.TenancyDomainId)?.Value;

        var isMainTenancy = user.HasClaim(CustomClaimTypes.IsMainTenancy, "True");

        // se o tenancyId da rota não bate com o do usuário, e ele não for MainTenancy, bloqueia
        if (!isMainTenancy && routeTenancyId is not null && routeTenancyId != userTenancyId)
        {
            //context.Result = new ForbidResult("Você não tem permissão para acessar esse domínio.");
            context.Result = new ObjectResult(new
            {
                message = "Você não tem permissão para acessar esse domínio."
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            return;
        }

        await next();
    }
}