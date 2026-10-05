namespace Approval.Application.Options;

public sealed class ApprovalRoutingOptions
{
    public const string SectionName = "ApprovalRouting";
    public decimal AmountThreshold { get; set; } = 1_000_000m;
}
