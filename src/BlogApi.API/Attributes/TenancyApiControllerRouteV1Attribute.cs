using Microsoft.AspNetCore.Mvc;

namespace BlogApi.API.Attributes;

public class TenancyApiControllerRouteV1Attribute : RouteAttribute
{
    public TenancyApiControllerRouteV1Attribute(string controller)
        : base($"api/v1/{{tenancyId}}/{controller}")
    {
    }
}
