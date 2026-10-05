using Approval.Application.Interfaces;
using Approval.Application.Services;
using Approval.Infrastructure.Persistence;
using Approval.Infrastructure.Repositories;
using Approval.Infrastructure.Identity;
using Approval.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Approval.Application.Options;

namespace Approval.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.Configure<ApprovalRoutingOptions>(configuration.GetSection(ApprovalRoutingOptions.SectionName));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IRoutingService, RoutingService>();
        return services;
    }
}
