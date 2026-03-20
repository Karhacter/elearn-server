# E-Learn Server

> **Backend REST API for a full-featured e-learning platform**, built with ASP.NET Core Web API (.NET 9), Entity Framework Core, SQL Server, and JWT authentication.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture & Project Structure](#architecture--project-structure)
- [API Reference](#api-reference)
- [Data Model](#data-model)
- [Getting Started](#getting-started)
- [Configuration Reference](#configuration-reference)
- [AI Recommendation Engine](#ai-recommendation-engine)
- [Development Notes](#development-notes)
- [Roadmap](#roadmap)
- [Documentation](#documentation)
- [License](#license)

---

## Overview

**E-Learn Server** is the server-side foundation of an online learning platform. It exposes a RESTful HTTP API consumed by a frontend client (Angular / Next.js). The API handles the full lifecycle of an e-learning product — from user registration and course browsing, to cart management, order placement, certificate issuance, and AI-assisted personalized course recommendations.

---

## Features

### 🔐 Authentication & Security
- JWT Bearer token authentication with configurable expiry
- Token blacklist support for secure logout
- Password hashing and salted storage
- Email-based forgot-password / reset-password flow

### 📚 Learning Domain
- Full CRUD for Courses with pagination, search, category filtering, and image uploads
- Category, Lesson, Enrollment, Rating, Comment, Certificate, Quiz, Assignment, and Notification APIs
- Wishlist management for bookmarking courses

### 🛒 Commerce Flow
- Shopping cart and cart item management
- Order creation, tracking, and order detail management
- Payment confirmation endpoint

### 🤖 AI-Powered Recommendations
- Roadmap-based course recommendation via a local [Ollama](https://ollama.com) model (`llama3.2:3b`)
- Automatic fallback to internal heuristic scoring when AI is unavailable
- Single endpoint: `POST /api/Recommendation/roadmap-courses`

### ⚙️ Infrastructure
- SQL Server persistence with EF Core code-first migrations
- OpenAPI document + Swagger UI available in development (`/swagger`)
- CORS policy for local frontend development
- Static file serving from `wwwroot`
- Circular reference handling in JSON serialization

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 9 |
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 9 |
| Database | Microsoft SQL Server |
| Authentication | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| API Documentation | NSwag / OpenAPI (`NSwag.AspNetCore 14`) |
| AI Integration | Ollama (local LLM, `llama3.2:3b`) |
| Email | SMTP via `IEmailService` |

---

## Architecture & Project Structure

```
elearn-server/
├── Controllers/          # API endpoint controllers (one per domain area)
├── Data/                 # AppDbContext and EF Core configuration
├── DTO/                  # Data Transfer Objects for request/response shaping
├── Migrations/           # EF Core auto-generated migration files
├── Models/               # Domain entity classes
├── Request/              # Specialized input request models
├── Security/             # JWT and token-related utilities
├── Services/             # Business logic — email, recommendations, etc.
├── docs/                 # CHANGELOG.md and WORKLOG.md
├── wwwroot/              # Statically served files (e.g., uploaded images)
├── Program.cs            # Application bootstrap, DI, middleware pipeline
├── appsettings.json      # Runtime configuration (connection strings, JWT, SMTP, Ollama)
└── elearn-server.csproj  # Project file and NuGet package references
```

---

## API Reference

The API is organized by domain controller. All routes are prefixed with `/api/`.

| Controller | Base Route | Responsibility |
|---|---|---|
| `AuthController` | `/api/Auth` | Register, login, logout, password reset |
| `UserController` | `/api/User` | User profile management |
| `CourseController` | `/api/Course` | Course CRUD, search, image upload |
| `CategoryController` | `/api/Category` | Course category management |
| `LessonController` | `/api/Lesson` | Lesson management per course |
| `CartController` | `/api/Cart` | Shopping cart and cart items |
| `OrderController` | `/api/Order` | Order creation and tracking |
| `OrderdetailController`| `/api/Orderdetail` | Line items within an order |
| `PaymentController` | `/api/Payment` | Payment confirmation |
| `EnrollmentController` | `/api/Enrollment` | Learner course enrollments |
| `CommentController` | `/api/Comment` | Course comments |
| `CertificateController`| `/api/Certificate` | Certificate issuance |
| `WishlistController` | `/api/Wishlist` | Learner wishlist |
| `RecommendationController` | `/api/Recommendation` | AI-powered course recommendations |

> **Swagger UI** is available at `/swagger` when running in the `Development` environment.

---

## Data Model

The database schema covers the following entities:

| Entity | Description |
|---|---|
| `User` | Platform accounts (learner / admin) |
| `Role` | Authorization roles |
| `Course` | Course metadata, pricing, and content |
| `Lesson` | Individual lessons belonging to a course |
| `Category` | Course categories/tags |
| `Enrollment` | Relation between a user and a course they've joined |
| `Rating` | Numeric ratings on courses |
| `Comment` | User comments on courses |
| `Certificate` | Issued after enrollment completion |
| `Quiz` / `Assignment` | Assessment entities |
| `Notification` | In-app user notifications |
| `Wishlist` | Saved courses per user |
| `Cart` / `CartItem` | Shopping cart state |
| `Order` / `OrderDetail` | Purchase records |
| `Payment` | Payment transaction records |
| `PasswordResetToken` | Time-limited tokens for password reset |
| `BlacklistedToken` | Revoked JWT tokens (logout blacklist) |

---

## Getting Started

### Prerequisites

- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (or SQL Server Express)
- _(Optional)_ [Ollama](https://ollama.com) for AI-powered recommendations

---

### 1. Clone the Repository

```bash
git clone https://github.com/Karhacter/elearn-server.git
cd elearn-server
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Application Settings

Edit `appsettings.json` (or create `appsettings.Development.json` to override locally):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=Elearn_db;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_HERE",
    "Issuer": "elearn-server",
    "Audience": "elearn-client",
    "ExpireDays": 7
  },
  "App": {
    "FrontendBaseUrl": "http://localhost:4200"
  },
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": "587",
    "Username": "user@example.com",
    "Password": "password",
    "FromEmail": "no-reply@example.com",
    "FromName": "E-Learn",
    "EnableSsl": "true"
  },
  "Ollama": {
    "BaseUrl": "http://localhost:11434/",
    "Model": "llama3.2:3b",
    "EnableRecommendations": true
  }
}
```

> ⚠️ **Never commit real secrets** to source control. Use environment variables or `appsettings.Development.json` (which should be git-ignored) to override sensitive values locally.

### 4. Apply Database Migrations

```bash
dotnet ef database update
```

### 5. Run the API

```bash
dotnet run
```

The API will start on `https://localhost:PORT`. In development mode, Swagger UI is served at `/swagger`.

---

## Configuration Reference

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `Jwt:Key` | Secret key for JWT signing (use a strong random value) |
| `Jwt:Issuer` | JWT issuer claim |
| `Jwt:Audience` | JWT audience claim |
| `Jwt:ExpireDays` | Token lifetime in days |
| `App:FrontendBaseUrl` | Origin URL of the frontend (used in CORS and email links) |
| `Smtp:Host` | SMTP server hostname |
| `Smtp:Port` | SMTP server port (default: 587) |
| `Smtp:Username` | SMTP authentication username |
| `Smtp:Password` | SMTP authentication password |
| `Smtp:FromEmail` | Sender email address |
| `Smtp:EnableSsl` | Whether to use TLS for SMTP |
| `Ollama:BaseUrl` | Base URL of the local Ollama server |
| `Ollama:Model` | Ollama model name to use for recommendations |
| `Ollama:EnableRecommendations` | Toggle AI recommendations on/off |

---

## AI Recommendation Engine

The recommendation module at `POST /api/Recommendation/roadmap-courses` accepts a `RoadmapName` string and returns an ordered list of suggested courses.

**Operating modes:**

| Mode | Trigger | Behavior |
|---|---|---|
| **AI Mode** | Ollama running and `EnableRecommendations: true` | Sends roadmap + course candidates to the local LLM for intelligent ranking |
| **Fallback Mode** | Ollama unavailable or disabled | Ranks courses using internal heuristic scoring rules |

**Setup (local Ollama):**

```bash
# Install and pull the model
ollama pull llama3.2:3b

# Start the Ollama server (if not running as a service)
ollama serve
```

Ensure `Ollama:BaseUrl` in `appsettings.json` points to your running Ollama instance.

---

## Development Notes

- **CORS**: The API currently allows requests from the origin specified in `App:FrontendBaseUrl` (default: `http://localhost:4200`). Extend the CORS policy for additional origins as needed.
- **Static files**: Uploaded assets (e.g., course images) are stored and served from `wwwroot/`.
- **JSON serialization**: `ReferenceHandler.IgnoreCycles` is configured globally to handle navigation property cycles in EF Core entities.
- **Merge conflicts**: `Program.cs` and `appsettings.json` currently contain unresolved Git merge conflict markers — these should be cleaned up before any production deployment.

---

## Roadmap

- [ ] Role-based authorization policies for admin and learner workflows
- [ ] Expanded automated test coverage (unit + integration)
- [ ] Externalize secrets via environment variables or a secrets manager
- [ ] Docker support (`Dockerfile` + `docker-compose.yml`)
- [ ] CI/CD pipeline documentation and GitHub Actions workflow
- [ ] Dedicated `Roadmap` entity/table to replace string-based roadmap input
- [ ] Rate limiting and request throttling middleware

---

## Documentation

| Document | Location | Purpose |
|---|---|---|
| Change History | [`docs/CHANGELOG.md`](docs/CHANGELOG.md) | Notable changes per session |
| Implementation Notes | [`docs/WORKLOG.md`](docs/WORKLOG.md) | Ongoing developer work log |

---

## License

This repository does not currently declare an open-source license. If you plan to distribute or reuse this project publicly, please add an appropriate license file (e.g., MIT, Apache 2.0).
