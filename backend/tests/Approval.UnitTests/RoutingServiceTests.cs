using Approval.Application.Options;
using Approval.Application.Services;
using Microsoft.Extensions.Options;
using Xunit;
namespace Approval.UnitTests;

public sealed class RoutingServiceTests
{
    private static RoutingService Create() => new(Options.Create(new ApprovalRoutingOptions { AmountThreshold = 1_000_000m }));

    [Fact]
    public void Amount_equal_to_threshold_goes_to_manager() => Assert.Equal("Manager", Create().ResolveAssignedRole(1_000_000m));

    [Fact]
    public void Amount_above_threshold_goes_to_finance() => Assert.Equal("Finance", Create().ResolveAssignedRole(1_000_000.01m));

    [Fact]
    public void Amount_below_threshold_goes_to_manager() => Assert.Equal("Manager", Create().ResolveAssignedRole(999_999m));
}
