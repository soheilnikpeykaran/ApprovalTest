namespace Approval.Application.Contracts;
public sealed record RegisterRequest(string Email,string Password,string FirstName,string LastName);
public sealed record LoginRequest(string Email,string Password);
public sealed record CreateRequestDto(string Title,decimal Amount,string? Description,string Urgency);
public sealed record RequestResponse(Guid Id,string Title,decimal Amount,string? Description,string Urgency,Approval.Domain.Entities.RequestStatus Status,string CreatedByUserId,string AssignedRole,DateTime CreatedAt,DateTime? DecisionAt);
