# ApprovalTest Backend - EnterpriseDesk Style

This backend replaces ASP.NET Core Identity/UserManager with the same layered approach used in EnterpriseDesk:

Controller -> Application Service -> Repository -> DbContext -> PostgreSQL

Authentication: UserRepository + BCrypt PasswordHasher + JwtTokenService.

Test accounts:
- employee@test.com / Employee123!
- manager@test.com / Manager123!
- finance@test.com / Finance123!

Run:
```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src\Approval.Api
```
