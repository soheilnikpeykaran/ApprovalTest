using Approval.Application.Interfaces;
namespace Approval.Infrastructure.Identity;

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => global::BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string passwordHash) => global::BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
