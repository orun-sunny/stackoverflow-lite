# StackOverflow Lite

`StackOverflow Lite` is a high-performance, simplified Q&A platform REST API built with **ASP.NET Core 10** using **Clean Architecture** principles. It features CQRS, validation middleware, secure JWT authentication, relational database persistence, and distributed caching capabilities.

---

## Technical Stack & Libraries

- **Framework**: ASP.NET Core 10 Web API
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Presentation/Host)
- **Patterns**: CQRS (Command Query Responsibility Segregation) via **MediatR**
- **Validation**: Input model validation via **FluentValidation**
- **Database**: PostgreSQL mapped with **Entity Framework Core 10**
- **Identity & Auth**: **ASP.NET Core Identity** for user storage & security + **JWT Bearer Authentication**
- **Caching**: **Distributed Caching** foundation (configured to fallback to Memory Cache; extensible to Redis)
- **Containerization**: **Docker** & **Docker Compose** for PostgreSQL, Redis, and Application orchestration

---

## Clean Architecture Directory Structure

The solution follows a strict separation of concerns, divided into four projects:

```
stackoverflow-lite/
├── StackOverflowLite.Domain/          # Core Domain Layer
│   ├── Entities/                      # Domain models (User, Question, Answer, Vote, Tag)
│   └── Enums/                         # Domain-specific enumerations (VoteType)
│
├── StackOverflowLite.Application/     # Application Logic Layer
│   ├── Core/                          # Core command/query abstractions & interfaces
│   ├── Dtos/                          # Data Transfer Objects
│   ├── Features/                      # Use cases sliced by Commands, Queries, and Handlers
│   └── Interfaces/                    # Infrastructure service abstractions (JWT, Repositories)
│
├── StackOverflowLite.Infrastructure/  # Infrastructure/Data Layer
│   ├── Data/                          # DbContext, Migrations, and Generic Repository pattern
│   └── Services/                      # Concrete services (JWT generator, health checks)
│
├── StackOverflowLite.Host/            # Presentation/API Entry Point
│   ├── Controllers/                   # Controller endpoints inheriting from BaseApiController
│   ├── Properties/                    # Launch profiles (launchSettings.json)
│   └── Program.cs                     # API bootstrap, middleware, and dependency injection pipeline
```

---

## Environment Variables & Configuration

The application is configured using standard ASP.NET Core configuration providers (`appsettings.json`, environment variables, user secrets).

| Configuration Key | Env Variable Override | Description | Default / Example Value |
|-------------------|-----------------------|-------------|-------------------------|
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | Connection string for PostgreSQL database. | `Host=localhost;Port=5432;Database=stackoverflow_lite;Username=postgres;Password=postgres` |
| `ConnectionStrings:Redis` | `ConnectionStrings__Redis` | Connection string for Redis. *(Falls back to standard In-Memory Distributed Cache if disabled)* | `localhost:6379` |
| `Jwt:Key` | `Jwt__Key` | Secret key used for signing JWT tokens. **Must be at least 32 characters long.** | `StackOverflowLite-Super-Secret-Key-Min-32-Chars!!` |
| `Jwt:ExpireInMinutes` | `Jwt__ExpireInMinutes` | Token validity duration in minutes. | `60` |
| `ASPNETCORE_ENVIRONMENT` | `ASPNETCORE_ENVIRONMENT` | Defines host execution mode (`Development` / `Production`). | `Development` |
| `ASPNETCORE_URLS` | `ASPNETCORE_URLS` | Binding URLs for the API server inside containers. | `http://+:8080` |

---

## How to Run Locally

### Prerequisites

- **.NET 10 SDK** (to build and run locally)
- **Docker & Docker Compose** (to run postgres and redis dependencies, or run the entire stack)

---

### Option 1: Run Infrastructure Dependencies in Docker, Run Code Locally (Recommended for Development)

1. **Spin up database and cache containers:**
   ```bash
   docker compose up postgres redis -d
   ```
   
2. **Apply migrations manually (Optional):**
   *Note: Database migrations apply automatically at application startup via the `DatabaseMigrationHostedService` background worker.*
   ```bash
   # Add new migrations (if making domain updates)
   dotnet ef migrations add <MigrationName> --project StackOverflowLite.Infrastructure --startup-project StackOverflowLite.Host
   
   # Update database manually
   dotnet ef database update --project StackOverflowLite.Infrastructure --startup-project StackOverflowLite.Host
   ```
   
3. **Run the API Host project:**
   ```bash
   dotnet run --project StackOverflowLite.Host
   ```

4. **Access the application:**
   - Swagger Documentation UI: `https://localhost:7287/swagger` or `http://localhost:5043/swagger`

---

### Option 2: Run the Entire Stack in Docker

1. **Build and start all services (PostgreSQL, Redis, and API Server):**
   ```bash
   docker compose up --build
   ```
   
2. **Access the containerized API:**
   - Swagger Documentation UI: `http://localhost:8080/swagger`

---

## API Reference Guide

### Authentication & Headers

Secure endpoints require standard **JWT Bearer Authentication**. Add the token to your HTTP request header as follows:
```http
Authorization: Bearer <your_jwt_token_here>
```

---

### Endpoint Summary

| Area | HTTP Method | Route | Description | Auth Required |
|------|-------------|-------|-------------|---------------|
| **Auth** | `POST` | `/api/auth/register` | Registers a new user account. | No |
| **Auth** | `POST` | `/api/auth/login` | Authenticates a user and returns a token. | No |
| **Auth** | `GET` | `/api/auth/profile` | Returns the profile of the current logged-in user. | **Yes** |
| **Questions** | `POST` | `/api/questions` | Creates a new question. | **Yes** |
| **Questions** | `GET` | `/api/questions` | Retrieves questions (optionally filter by `tag`). | No |
| **Questions** | `GET` | `/api/questions/{id}` | Retrieves a single question by its ID. | No |
| **Questions** | `PUT` | `/api/questions/{id}` | Updates a question's title, body, or tags. | **Yes** |
| **Questions** | `DELETE` | `/api/questions/{id}` | Deletes a question. | **Yes** |
| **Answers** | `POST` | `/api/answers` | Adds an answer to a question. | **Yes** |
| **Answers** | `GET` | `/api/questions/{questionId}/answers` | Lists all answers for a question. | No |
| **Answers** | `PUT` | `/api/answers/{id}` | Updates an answer's content. | **Yes** |
| **Answers** | `DELETE` | `/api/answers/{id}` | Deletes an answer. | **Yes** |
| **Answers** | `PUT` | `/api/questions/{questionId}/accept-answer` | Accepts an answer (or un-accepts if null). | **Yes** |
| **Votes** | `POST` | `/api/votes` | Casts an Upvote or Downvote on a question or answer. | **Yes** |

---

## 📡 API Overview

### 🔹 Auth

* `POST /api/auth/register` → Register user
* `POST /api/auth/login` → Login & get token
* `GET /api/auth/profile` → Get user profile

---

### 🔹 Questions

* `POST /api/questions` → Create question
* `GET /api/questions` → List questions *(filter by tag)*
* `GET /api/questions/{id}` → Get single question
* `PUT /api/questions/{id}` → Update question
* `DELETE /api/questions/{id}` → Delete question

---

### 🔹 Answers

* `POST /api/answers` → Add answer
* `GET /api/questions/{id}/answers` → Get answers
* `PUT /api/answers/{id}` → Update answer
* `DELETE /api/answers/{id}` → Delete answer
* `PUT /api/questions/{id}/accept-answer` → Accept answer

---

### 🔹 Votes

* `POST /api/votes` → Upvote / Downvote






