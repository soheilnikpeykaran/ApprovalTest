using Approval.Domain.Entities;

namespace Approval.Application.Contracts;

public sealed record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public sealed record LoginRequest(string Email, string Password);
public sealed record AuthResponse(string Token, DateTime ExpiresAt, string[] Roles, string UserId);
public sealed record CreateRequestDto(string Title, decimal Amount, string? Description, string Urgency);
public sealed record RequestResponse(Guid Id, string Title, decimal Amount, string? Description, string Urgency, RequestStatus Status, string CreatedByUserId, string AssignedRole, DateTime CreatedAt, DateTime? DecisionAt);
public sealed record DecisionDto(string Action);
