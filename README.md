# 🎓 E-Learn Server

> **Professional RESTful API for a modern e-learning platform**, architected with **Clean Architecture** and built on **.NET 9**.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet)
[![EF Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![JWT](https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=JSON%20web%20tokens&logoColor=white)](https://jwt.io/)
[![Ollama](https://img.shields.io/badge/Ollama-AI_Recs-000000?style=for-the-badge&logo=ollama&logoColor=white)](https://ollama.com/)

---

## 📖 Table of Contents

- [🌟 Project Highlight](#-project-highlight)
- [✨ Key Features](#-key-features)
- [🛠️ Tech Stack](#️-tech-stack)
- [🏗️ Architecture & Structure](#️-architecture--structure)
- [🔌 API Reference](#-api-reference)
- [📂 Data Model](#-data-model)
- [🚀 Getting Started](#-getting-started)
- [⚙️ Configuration](#️-configuration)
- [🤖 AI Recommendation Engine](#-ai-recommendation-engine)
- [📝 Documentation & Logs](#-documentation--logs)

---

## 🌟 Project Highlight

**E-Learn Server** provides a robust, scalable backend for online learning platforms. It handles the entire learner journey—from course discovery and enrollment to AI-assisted roadmap generation and certificate issuance. Designed with **domain-driven principles**, it ensures high maintainability and testability.

---

## ✨ Key Features

### 🔐 Security & Auth
- **JWT Authentication**: Secure Bearer token flow with configurable expiration.
- **Smart Security**: Password hashing, email verification, and token blacklisting for safe logouts.
- **Rate Limiting**: Brute-force protection on critical endpoints like Login and Forgot Password.

### 📚 Learning Operations
- **Course Catalog**: Rich CRUD operations with multi-level categories, advanced search, and pagination.
- **Content Delivery**: Management of Lessons, Quizzes, Assignments, and Multimedia uploads.
- **User Progress**: Enrollment tracking, certificates for graduates, and real-time notifications.

### 🛒 E-Commerce Integration
- **Shopping Flow**: Persistent Cart and Wishlist management.
- **Orders & Payments**: Comprehensive order lifecycle tracking with payment status verification.

### 🤖 AI-Powered Experience
- **Smart Roadmaps**: Leveraging local LLMs (via Ollama) to suggest personalized learning paths.
- **Heuristic Fallback**: Intelligent course ranking even when AI models are offline.

---

## 🏗️ Architecture & Structure

This project follows **Clean Architecture** (Onion Architecture) to decouple business logic from infrastructure concerns.

### 📂 Directory Layout
```text
elearn-server/
├── 📂 src/
│   ├── 🏛️ Domain/          # core entities, value objects, & repository interfaces
│   ├── ⚡ App/             # usage scenarios (Services), DTOs, & mapping logic
│   ├── ⚙️ Infrastructure/   # database (EF Core), AI (Ollama), & external services
│   └── 🌐 Presentation/     # REST controllers & custom middleware
├── 🎬 Program.cs            # application entry point & DI configuration
├── 📄 appsettings.json      # runtime settings & secrets
└── 🧪 elearn-server.csproj  # project dependencies
```

---

## 🔌 API Reference

The API is structured by domain areas. All endpoints are versioned under `/api/`.

| Area | Controller | Responsibility |
|---|---|---|
| **Identity** | `Auth` | Registration, Login, Security |
| **Users** | `User` | Profile & Account management |
| **Catalog** | `Course`, `Category` | Discovery & Content management |
| **Learning** | `Lesson`, `Enrollment` | Teaching & Learning flows |
| **Orders** | `Cart`, `Order`, `Payment`| Checkout & Transaction records |
| **Engagement** | `Comment`, `Wishlist`, `Cert` | Social & Gamification |
| **AI** | `Recommendation` | Intelligent Course Roadmaps |

> 🛠️ **Interactive Docs**: Swagger/OpenAPI UI is available at `/swagger` in **Development** mode.

---

## 📂 Data Model

| Entity | Domain |
|---|---|
| `User`, `Role` | Identity |
| `Course`, `Lesson`, `Category`| Knowledge |
| `Enrollment`, `Certificate` | Learner Progress |
| `Quiz`, `Assignment` | Assessment |
| `Cart`, `Order`, `Payment` | Commerce |
| `Comment`, `Rating`, `Notification` | Social |

---

## 🚀 Getting Started

### 📋 Prerequisites
- **.NET 9 SDK**
- **SQL Server** (LocalDB or Docker instance)
- **Ollama** (optional, for AI features)

### ⚙️ Quick Start
1. **Clone the project:** `git clone https://github.com/Karhacter/elearn-server.git`
2. **Update Connection String:** Edit `appsettings.json` with your SQL Server details.
3. **Apply Migrations:**
   ```bash
   dotnet ef database update
   ```
4. **Run the App:**
   ```bash
   dotnet run
   ```

---

## ⚙️ Configuration Reference

| Key | Example / Default |
|---|---|
| `Jwt:Key` | Strong HMAC-SHA256 Secret |
| `App:FrontendBaseUrl` | `http://localhost:4200` (for CORS) |
| `Ollama:Model` | `llama3.2:3b` |
| `Smtp:Host` | Your Mail Server |

---

## 🤖 AI Recommendation Engine

Access intelligent roadmaps via: `POST /api/Recommendation/roadmap-courses`

**Setup Local AI:**
```bash
ollama pull llama3.2:3b
ollama serve
```

---

## 📝 Documentation & Logs

- 📄 [Change History](docs/CHANGELOG.md)
- 🛠️ [Implementation Notes](docs/WORKLOG.md)

---

## ⚖️ License

Project is currently under internal ownership. See future updates for open-source licensing.
