# Online Voting System

An ASP.NET Core MVC web application for conducting secure online elections. Voters register and authenticate via JWT, cast ballots through a web interface, and admins manage elections through a dedicated admin panel.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| Language | C# |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer (stored in HttpOnly cookie) |
| Password Hashing | BCrypt.Net-Next |
| Frontend | HTML, SCSS, JavaScript, Bootstrap |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code with the C# extension

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/basiratabidi/OnlineVotingSystem.git
cd OnlineVotingSystem
```

### 2. Configure the database connection

Open `appsettings.json` and update the connection string to point to your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=OnlineVotingSystem;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Configure JWT settings

> ⚠️ **Security notice:** Before running the application, replace the hardcoded JWT secret in `Program.cs` with a value stored in `appsettings.json` or an environment variable. Never commit real secrets to source control.

```json
{
  "Jwt": {
    "Key": "your-strong-secret-key-here",
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourdomain.com"
  }
}
```

### 4. Apply database migrations

The application runs EF Core migrations automatically on startup. Alternatively, apply them manually:

```bash
dotnet ef database update
```

### 5. Run the application

```bash
dotnet run
```

Navigate to `https://localhost:5106` in your browser.

---

## Default Admin Account

An admin account is seeded automatically on first run:

| Field | Value |
|---|---|
| Email | `admin@gmail.com` |
| Password | `admin@123` |

> ⚠️ **Change the admin password immediately after first login**, especially before any deployment.

---

## Project Structure

```
OnlineVotingSystem/
├── Controllers/          # MVC controllers (Voters, Admin, etc.)
├── Models/
│   ├── Entities/         # EF Core entity classes (User, Vote, etc.)
│   └── Helper/           # Helper classes (e.g. EmailHelper)
├── Views/                # Razor views (.cshtml)
├── Migrations/           # EF Core database migrations
├── wwwroot/              # Static assets (CSS, JS, Bootstrap)
├── Program.cs            # App entry point, service registration, middleware
├── appsettings.json      # Configuration (connection strings, JWT)
└── OnlineVotingSystem.csproj
```

---

## Key Dependencies

| Package | Version | Purpose |
|---|---|---|
| `BCrypt.Net-Next` | 4.0.3 | Password hashing |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.20 | JWT authentication |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.20 | SQL Server ORM |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.20 | EF Core CLI tooling |

---

## Known Limitations

- JWT issuer/audience are hardcoded to `localhost` — must be updated for any deployment
- No HTTPS redirection middleware configured
- `SQLQuery1.sql` in the repo root is empty and can be removed
- `Portable.BouncyCastle` is listed as a dependency but is unused; only `BCrypt.Net-Next` is used for hashing

---

## License

This project is for educational purposes.
