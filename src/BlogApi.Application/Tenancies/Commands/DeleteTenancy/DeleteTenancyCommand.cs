using MediatR;

namespace BlogApi.Application.Tenancies.Commands.DeleteTenancy;

public class DeleteTenancyCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Url { get; set; }
}
