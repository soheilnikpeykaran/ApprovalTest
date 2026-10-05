using Approval.Application.Interfaces;
using Approval.Application.Options;
using Microsoft.Extensions.Options;

namespace Approval.Application.Services;

public sealed class RoutingService(IOptions<ApprovalRoutingOptions> options) : IRoutingService
{
    private readonly ApprovalRoutingOptions _options = options.Value;

    public string ResolveAssignedRole(decimal amount)
    {
        return amount <= _options.AmountThreshold ? "Manager" : "Finance";
    }
}
