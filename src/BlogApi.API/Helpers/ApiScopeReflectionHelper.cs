using BlogApi.API.Attributes;
using BlogApi.Application.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace BlogApi.API.Helpers;

public static class ApiScopeReflectionHelper
{
    public static List<ApiScope> ExtractApiScopes(string baseUrl)
    {
        var result = new List<ApiScope>();

        var controllerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ControllerBase)) && !t.IsAbstract);

        foreach (var controller in controllerTypes)
        {
            var controllerRoute = controller
                .GetCustomAttributes(inherit: true)
                .OfType<RouteAttribute>()
                .FirstOrDefault()?.Template ?? "";

            var tenancyPrefix = controller
                .GetCustomAttributes(inherit: true)
                .OfType<TenancyApiControllerRouteV1Attribute>()
                .FirstOrDefault()?.Template ?? "";

            foreach (var method in controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var scopeAttr = method.GetCustomAttributes(true)
                    .OfType<RequireApiScopeAttribute>()
                    .FirstOrDefault();

                if (scopeAttr == null)
                    continue;

                var httpMethodAttr = method.GetCustomAttributes(true)
                    .FirstOrDefault(attr => attr is HttpMethodAttribute) as HttpMethodAttribute;

                var methodTemplate = httpMethodAttr?.Template ?? "";
                var verb = httpMethodAttr?.HttpMethods?.FirstOrDefault()?.ToUpper() ?? "GET";

                var baseURL = baseUrl.TrimEnd('/');
                var fullRoute = $"{baseURL}/{tenancyPrefix}".TrimEnd('/');

                if (!string.IsNullOrEmpty(methodTemplate))
                {
                    fullRoute += "/" + methodTemplate.TrimStart('/');
                }

                result.Add(new ApiScope
                {
                    Name = $"{scopeAttr.Scope} | {verb} {fullRoute}"
                });
            }
        }

        return result;
    }
}
