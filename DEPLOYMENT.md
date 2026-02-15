# 🚀 Deployment Guide - InfraMonitor

This guide provides step-by-step instructions for deploying the **InfraMonitor** system in differing environments (Development, UAT, Production) using Docker.

---

## 📋 Prerequisites

Before starting, ensure the host machine has the following installed:

1.  **Docker Desktop** (or Docker Engine + Compose plugin on Linux)
2.  **Git** (to clone the repository)
3.  **.NET 8.0 SDK** (Optional, for manual database migrations if not running via container)

---

## 🐳 Docker Composition

The project uses multiple `docker-compose` files to manage different environments:

| File | Environment | Description |
| :--- | :--- | :--- |
| `docker-compose.yml` | **Development** | Default setup. Exposes ports for local debugging. Includes SQL Server, Postgres, Redis, and API. |
| `docker-compose.prod.yml` | **Production** | Optimized for security and performance. Uses environment variables for secrets. Restart policies enabled. |
| `docker-compose.uat.yml` | **UAT** | User Acceptance Testing configuration (mirrors Prod but often with different data fixtures). |

---

## 🛠️ Deploying to Development (Local)

1.  **Clone the Repository**:
    ```bash
    git clone https://github.com/yourusername/InfraMonitor.git
    cd InfraMonitor
    ```

2.  **Start Services**:
    Run the default compose file to spin up the entire stack.
    ```bash
    docker-compose up -d --build
    ```

3.  **Access the Application**:
    *   **API (Swagger)**: [https://localhost:4434/swagger](https://localhost:4434/swagger)
    *   **Frontend**: [http://localhost:4201](http://localhost:4201)
    *   **Hangfire Dashboard**: [https://localhost:4434/hangfire](https://localhost:4434/hangfire)

4.  **Database Initialization**:
    The system includes an `init.sh` script that attempts to run `001_InitialSchema.sql` for SQL Server. However, for full EF Core migrations, run:
    ```bash
    dotnet ef database update --project InfraMonitor.Infrastructure --startup-project InfraMonitor.WebAPI
    ```

---

## 🚀 Deploying to Production

For production, we use `docker-compose.prod.yml` which expects secure credentials via environment variables.

### 1. Set Environment Variables
**Linux/Mac**:
```bash
export PROD_SQL_PASSWORD="YourStrongPassword123!"
export PROD_LOGS_PASSWORD="YourLogPassword123!"
```

**Windows (PowerShell)**:
```powershell
$env:PROD_SQL_PASSWORD="YourStrongPassword123!"
$env:PROD_LOGS_PASSWORD="YourLogPassword123!"
```

### 2. Deploy Stack
Combine the base config with the production override:
```bash
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

### 3. Verify Deployment
*   **API Health**: `http://your-server-ip/health`
*   **Logs**: Check container logs to ensure no startup errors.
    ```bash
    docker logs inframonitor_webapi_1
    ```

---

## ⚙️ Configuration Details

The application is configured via environment variables mapped in `docker-compose`. Key variables include:

| Variable | Description |
| :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | Sets the runtime environment (`Development`, `Staging`, `Production`). |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string. |
| `ConnectionStrings__LogDatabase` | PostgreSQL connection string for Serilog. |
| `ConnectionStrings__Redis` | Redis connection string. |

---

## 🔄 Database Migrations (Production)

In production, it is recommended to apply migrations via a generated SQL script or a dedicated migration container/job.

**Generate Script**:
```bash
dotnet ef migrations script --output migration.sql --project InfraMonitor.Infrastructure --startup-project InfraMonitor.WebAPI
```
Then execute `migration.sql` on your production SQL Server instance.

---

## 🐞 Troubleshooting

*   **SQL Server Connectivity**: Ensure the `SA` password meets complexity requirements (Uppercase, Lowercase, Number, Symbol). If the password is too weak, the container will silently exit.
*   **Redis Connection Refused**: Ensure the API container can resolve the `redis` hostname (check docker network).
*   **Docker Volume Permissions**: If Postgres fails to start with "Permission denied", check directory permissions on the host `./Database/logs-data` folder.
