# EF Core Migrations

This document describes how to manage Entity Framework Core migrations for
`Unjai.Platform.Infrastructure` (Class Library) using PostgreSQL and dotnet Aspire.

---

## Prerequisites

- .NET SDK
- PostgreSQL
- EF Core CLI

### Install EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
# Or update to the latest version if already installed
dotnet tool update --global dotnet-ef
```

### Environment Variable:
Migrations use a design-time DbContext factory and read the connection string from an environment variable.
```bash
export POSTGRES_CONNECTIONSTRING="Host=localhost;Port=5432;Username=postgres;Password=postgres"
```

### Add Migration
```bash
dotnet ef migrations add Initial \
  --project ./Src/Unjai.Platform.Infrastructure/Unjai.Platform.Infrastructure.csproj \
  --context AppDbContext
```

### Remove Migration
```bash
dotnet ef migrations remove \
  --project ./Src/Unjai.Platform.Infrastructure/Unjai.Platform.Infrastructure.csproj \
  --context AppDbContext
```

### Generate SQL Script (Idempotent)
```bash
dotnet ef migrations script \
  --project ./Src/Unjai.Platform.Infrastructure/Unjai.Platform.Infrastructure.csproj \
  --context AppDbContext \
  --idempotent \
  -o ./Src/.container/postgres/initdb/20260125072850_Initial.sql
```
