using Approval.Application.Interfaces;
using Approval.Application.Options;
using Approval.Application.Services;
using Approval.Infrastructure.Identity;
using Approval.Infrastructure.Persistence;
using Approval.Infrastructure.Repositories;
using Approval.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Approval.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        var thresholdValue =
     configuration[$"{ApprovalRoutingOptions.SectionName}:AmountThreshold"];

        if (!decimal.TryParse(thresholdValue, out var threshold))
        {
            throw new InvalidOperationException(
                "ApprovalRouting:AmountThreshold is missing or invalid.");
        }

        services.Configure<ApprovalRoutingOptions>(options =>
        {
            options.AmountThreshold = threshold;
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRoutingService, RoutingService>();
        services.AddScoped<IRequestService, RequestService>();

        return services;
    }
}