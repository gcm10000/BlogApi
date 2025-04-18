using BlogApi.Application.Tenancies.Dtos;
using MediatR;

namespace BlogApi.Application.Tenancies.Commands.UpdateTenancy;

public class UpdateTenancyCommand : IRequest<TenancyDto>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AdministratorEmail { get; set; }
    public string Url { get; set; }
}
