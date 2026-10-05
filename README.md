# Full-Stack Approval Workflow

A small enterprise-style approval workflow built with ASP.NET Core 9, EF Core, PostgreSQL and React/Vite.

## Architecture

Backend uses a simple layered structure:

- `Approval.Domain`: domain entities/enums.
- `Approval.Application`: DTOs, business services, routing rule, repository/UoW contracts and application exceptions.
- `Approval.Infrastructure`: EF Core, PostgreSQL, ASP.NET Core Identity, JWT creation, repositories and seed data.
- `Approval.Api`: HTTP controllers, authentication/authorization, Swagger and error handling.

Frontend uses React with a schema-driven form. The renderer is generic: changing the schema's fields does not require changing the rendering code.

## Business rule

`ApprovalRouting:AmountThreshold` is configurable in `appsettings.json`.

- Amount <= threshold -> `Manager`
- Amount > threshold -> `Finance`

The decision is performed on the server; the client cannot choose `AssignedRole`.

## Authorization rules

- Registration always creates an `Employee`.
- `Employee` can create requests.
- Request list is filtered on the server: a user sees requests they created OR requests assigned to one of their roles.
- A decision is allowed only when the current user's role equals the request's `AssignedRole` and the request is still `Pending`.

## Demo users

Seeded automatically:

| Role | Email | Password |
|---|---|---|
| Employee | employee@test.com | Employee123! |
| Manager | manager@test.com | Manager123! |
| Finance | finance@test.com | Finance123! |

These credentials are for local evaluation only.

## Run backend

Requirements: .NET 9 SDK, Docker Desktop, PostgreSQL via Docker.

From `backend`:

```powershell
docker compose up -d postgres
dotnet restore
dotnet tool update --global dotnet-ef --version 9.0.10
dotnet ef migrations add InitialCreate --project .\src\Approval.Infrastructure --startup-project .\src\Approval.Api --output-dir Migrations
dotnet ef database update --project .\src\Approval.Infrastructure --startup-project .\src\Approval.Api
dotnet run --project .\src\Approval.Api --launch-profile https
```

If the migration already exists in the repository, skip the `migrations add` command and run only `database update`.

Swagger: `https://localhost:7001/swagger`

## Run frontend

```powershell
cd frontend
npm install
$env:VITE_API_URL="https://localhost:7001/api"
npm run dev
```

Open the Vite URL shown in the terminal.

If the browser rejects the local HTTPS certificate, trust the ASP.NET Core development certificate:

```powershell
dotnet dev-certs https --trust
```

## API endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/form-schema`
- `POST /api/requests` — Employee only
- `GET /api/requests` — authenticated users; server-side filtering
- `POST /api/requests/{id}/decision` — assigned role only

## Example request

```json
{
  "title": "خرید لپ‌تاپ",
  "amount": 1500000,
  "description": "لپ‌تاپ برای واحد فنی",
  "urgency": "زیاد"
}
```

Because `1500000 > 1000000`, the server assigns the request to `Finance`.

## State-management decision

The frontend uses local React state (`useState`) and `useEffect` because this application has a small number of screens and no complex cross-page state graph. Adding Redux or another state library would increase complexity without solving a current problem.

The JWT is stored in `localStorage` to satisfy the exercise's requirement to keep the token for subsequent requests. For a production system, I would prefer short-lived access tokens plus refresh-token rotation in a secure HttpOnly cookie, depending on the application's threat model.

## What was intentionally kept simple

- No Autofac: the built-in ASP.NET Core DI container is sufficient here.
- No JSONB bonus: the request fields are fixed domain properties; adding JSONB would not improve this exercise's core workflow.
- No refresh-token endpoint: the assignment explicitly requires JWT login, not a complete refresh-token subsystem.

## Tests

The routing rule is isolated behind `IRoutingService`, making it straightforward to unit test without a database.

Run:

```powershell
dotnet test
```
