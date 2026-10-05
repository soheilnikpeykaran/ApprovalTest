namespace Approval.Domain.Entities;

public enum RequestStatus { Pending, Approved, Rejected }

public sealed class Request
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Urgency { get; set; } = string.Empty;
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public string CreatedByUserId { get; set; } = string.Empty;
    public string AssignedRole { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? DecisionAt { get; set; }
    public string? DecisionByUserId { get; set; }
}
