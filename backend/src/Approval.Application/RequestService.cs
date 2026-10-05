using Approval.Application.Contracts;
using Approval.Application.Interfaces;
using Approval.Domain.Entities;

namespace Approval.Application.Services;

public sealed class RequestService(
    IRepository<Request> repository,
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    IRoutingService routingService) : IRequestService
{
    public async Task<RequestResponse> CreateAsync(string userId, CreateRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var creatorId)) throw new InvalidOperationException("Invalid user id.");
        var entity = new Request
        {
            Id = Guid.NewGuid(), Title = dto.Title.Trim(), Amount = dto.Amount,
            Description = dto.Description, Urgency = dto.Urgency.Trim(),
            CreatedByUserId = creatorId, AssignedRole = routingService.ResolveAssignedRole(dto.Amount),
            CreatedAt = DateTime.UtcNow, Status = RequestStatus.Pending
        };
        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<IReadOnlyList<RequestResponse>> GetVisibleAsync(string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var currentUserId)) throw new InvalidOperationException("Invalid user id.");
        var items = await requestRepository.GetVisibleAsync(currentUserId, roles, cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<RequestResponse> DecideAsync(Guid requestId, string userId, IReadOnlyCollection<string> roles, string action, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var decisionUserId)) throw new InvalidOperationException("Invalid user id.");
        var entity = await repository.GetByIdAsync(requestId, cancellationToken) ?? throw new KeyNotFoundException("Request not found.");
        if (entity.Status != RequestStatus.Pending) throw new InvalidOperationException("Only pending requests can be decided.");
        if (!roles.Contains(entity.AssignedRole)) throw new UnauthorizedAccessException("You are not allowed to decide this request.");
        entity.Status = action.Trim().ToLowerInvariant() switch
        {
            "approve" => RequestStatus.Approved,
            "reject" => RequestStatus.Rejected,
            _ => throw new ArgumentException("Action must be approve or reject.")
        };
        entity.DecisionByUserId = decisionUserId;
        entity.DecisionAt = DateTime.UtcNow;
        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private static RequestResponse Map(Request x) => new(x.Id, x.Title, x.Amount, x.Description, x.Urgency, x.Status, x.CreatedByUserId.ToString(), x.AssignedRole, x.CreatedAt, x.DecisionAt);
}
