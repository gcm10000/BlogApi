using BlogApi.Application.Tenancies.Dtos;
using MediatR;

namespace BlogApi.Application.Tenancies.Commands.CreateTenancy;

public class CreateTenancyCommand : IRequest<TenancyDto>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Url { get; set; }
}
