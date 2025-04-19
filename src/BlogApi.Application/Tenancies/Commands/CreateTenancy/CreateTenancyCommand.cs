using BlogApi.Application.Tenancies.Dtos;
using MediatR;

namespace BlogApi.Application.Tenancies.Commands.CreateTenancy;

public class CreateTenancyCommand : IRequest<TenancyDto>
{
    public string Name { get; set; }
    public string MainAdministratorEmail { get; set; }
    public string Url { get; set; }
}
