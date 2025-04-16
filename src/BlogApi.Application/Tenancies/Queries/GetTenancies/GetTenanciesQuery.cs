using BlogApi.Application.Common;
using BlogApi.Application.Tenancies.Dtos;
using MediatR;

namespace BlogApi.Application.Tenancies.Queries.GetTenancies;

public class GetTenanciesQuery : IRequest<PagedResponse<List<TenancyDto>>>
{
    public string Name { get; set; }
    public int PageNumber { get; set; } = 1; // Valor padrão 1 para a página
    public int PageSize { get; set; } = 10;  // Valor padrão 10 itens por página
}