using BlogApi.Application.ApiKeys.Dto;
using MediatR;

namespace BlogApi.Application.ApiKeys.Queries.GetApiKey;
public class GetApiKeyQuery : IRequest<ApiKeyDto?>
{
    public int Id { get; set; }

    public GetApiKeyQuery(int id)
    {
        Id = id;
    }

}
