using BlogApi.Application.Tenancies.Dtos;
using MediatR;

namespace BlogApi.Application.Tenancies.Queries.GetTenancyById;

public class GetTenancyByIdQuery : IRequest<TenancyDto>
{
    public int Id { get; set; }
}
