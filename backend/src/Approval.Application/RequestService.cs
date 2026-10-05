using Approval.Application.Contracts;
using Approval.Application.Exceptions;
using Approval.Application.Interfaces;
using Approval.Domain.Entities;

namespace Approval.Application.Services;

public sealed class RequestService(IRepository<Request> repository, IUnitOfWork unitOfWork, IRoutingService routingService) : IRequestService
{
    public async Task<RequestResponse> CreateAsync(string userId, CreateRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Amount <= 0) throw new AppException("Amount must be greater than zero.", 400);
        if (string.IsNullOrWhiteSpace(dto.Title)) throw new AppException("Title is required.", 400);
        if (string.IsNullOrWhiteSpace(dto.Urgency)) throw new AppException("Urgency is required.", 400);

        var entity = new Request
        {
            Id = Guid.NewGuid(), Title = dto.Title.Trim(), Amount = dto.Amount,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            Urgency = dto.Urgency.Trim(), Status = RequestStatus.Pending,
            CreatedByUserId = userId, AssignedRole = routingService.ResolveAssignedRole(dto.Amount),
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<IReadOnlyList<RequestResponse>> GetVisibleAsync(string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default)
    {
        var roleSet = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var query = repository.Query().Where(x => x.CreatedByUserId == userId || roleSet.Contains(x.AssignedRole));
        var entities = await repository.ToListAsync(query.OrderByDescending(x => x.CreatedAt), cancellationToken);
        var items = entities.Select(Map).ToList();
        return items;
    }

    public async Task<RequestResponse> DecideAsync(Guid requestId, string userId, IReadOnlyCollection<string> roles, string action, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(requestId, cancellationToken) ?? throw new AppException("Request not found.", 404);
        if (!roles.Any(r => string.Equals(r, entity.AssignedRole, StringComparison.OrdinalIgnoreCase)))
            throw new AppException("You are not allowed to decide this request.", 403);
        if (entity.Status != RequestStatus.Pending) throw new AppException("Only pending requests can be decided.", 409);

        var normalized = action.Trim().ToLowerInvariant();
        entity.Status = normalized switch
        {
            "approve" => RequestStatus.Approved,
            "reject" => RequestStatus.Rejected,
            _ => throw new AppException("Action must be approve or reject.", 400)
        };
        entity.DecisionAt = DateTime.UtcNow;
        entity.DecisionByUserId = userId;
        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private static RequestResponse Map(Request x) => new(x.Id, x.Title, x.Amount, x.Description, x.Urgency, x.Status, x.CreatedByUserId, x.AssignedRole, x.CreatedAt, x.DecisionAt);
}
