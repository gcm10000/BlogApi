using BlogApi.Application.Infrastructure.Identity.Models;
using MediatR;

namespace BlogApi.Application.ApiScopes;
public class GetAllApiScopesQuery : IRequest<List<ApiScope>>
{
}