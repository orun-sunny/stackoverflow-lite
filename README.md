# StackOverflow Lite

Simplified Q&A platform REST API built with ASP.NET Core Clean Architecture.

## Stack

- ASP.NET Core 10
- Clean Architecture (Domain, Application, Infrastructure, Host)
- MediatR + FluentValidation + ErrorOr
- Entity Framework Core + PostgreSQL
- ASP.NET Identity + JWT
- Redis (caching foundation)
- Docker Compose

## Project structure

```
StackOverflowLite.Domain/          Entities and domain rules
StackOverflowLite.Application/     Commands, queries, handlers, DTOs
StackOverflowLite.Infrastructure/  EF Core, Identity, Redis, repositories
StackOverflowLite.Host/            API entry point, controllers, Swagger
```

## Run locally

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- Redis

### Steps

```bash
# Start dependencies (or use docker compose for postgres + redis only)
docker compose up postgres redis -d

# Run API
dotnet run --project StackOverflowLite.Host
```

Swagger UI (Development): `https://localhost:7287/swagger` or `http://localhost:5043/swagger`

Health check: `GET /api/health`

## Run with Docker

```bash
docker compose up --build
```

API: `http://localhost:8080/swagger`

## Environment variables

| Key | Description |
|-----|-------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `ConnectionStrings__Redis` | Redis connection string |
| `Jwt__Key` | JWT signing key (min 32 characters) |
| `Jwt__ExpireInMinutes` | Token lifetime in minutes |

## Migrations

```bash
dotnet ef migrations add <Name> --project StackOverflowLite.Infrastructure --startup-project StackOverflowLite.Host
dotnet ef database update --project StackOverflowLite.Infrastructure --startup-project StackOverflowLite.Host
```

Migrations also run automatically on application startup.

## API (planned)

| Area | Endpoints |
|------|-----------|
| Auth | Register, Login, Profile |
| Questions | CRUD, filter by tag |
| Answers | CRUD, list by question |
| Votes | Upvote/downvote questions and answers |
| Tags | Create, list |

## Commit guide

See [COMMITS.md](./COMMITS.md) for the step-by-step commit plan.
