
# SampleProject

🚀 **SampleProject** is a .NET 9 Web API solution built using Clean Architecture principles and equipped with powerful logging, idempotency, containerization, and observability features. It's designed to be scalable, maintainable, and production-ready out of the box.

---

## ⭐ Star This Repository!

---

## 🔧 What's Inside?

- **Framework & Architecture**
  - ASP.NET Core 9 Web API
  - Clean Architecture with separate concerns
  - `Minimal API` structure using top-level statements

- **Core Features**
  - **Custom Idempotency** with `[Idempotent]` attribute
  - **Structured Logging** with Serilog (JSON config-based)
  - **Swagger/OpenAPI** integration
  - **HTTP Request Logging** with enrichment
  - **Exception Handling Middleware**
  - **Environment-Specific Configuration**

- **Logging**
  - Configurable Serilog sinks via `seri-log.config.json`
  - File-based log output (log rotation by date)
  - Correlation ID support for traceability

- **Configuration & Environment**
  - Supports multiple environments (e.g., `Development`, `Production`)
  - Separate config files (`appsettings.json`, `appsettings.Development.json`)
  - External Serilog config binding

- **API Utilities**
  - HTTP request pipeline logging
  - Health check endpoint
  - Versioning-ready structure

---

## 🧪 Testing & Quality

- Extensible attribute-based design
- Easily testable components via DI and Middleware
- Logging and idempotency integrated at the controller level

---

## 🐳 Docker & DevOps

- **Docker Support**
  - Includes `Dockerfile` and `docker-compose.yml`
  - Can containerize the API and log infrastructure

- **.dockerignore** included for clean builds

---

## 📝 How to Run

1. **Clone the Repository**
   ```bash
   git clone https://github.com/your-username/SampleProject.git
   cd SampleProject
   ```

2. **Build & Run (Docker)**
   ```bash
   docker-compose up --build
   ```

3. **Run Locally (.NET CLI)**
   ```bash
   cd src/SampleProject.API
   dotnet run
   ```

4. **Access API**
   - Swagger UI: `https://localhost:5001/swagger`
   - Logs: `src/logs/webapi-<date>.log`

---

## 📬 Contributions

Pull requests are welcome! If you spot an improvement or a bug, feel free to contribute.

---

## Stay Connected

- **GitHub**: [Sameer Ulhaq](https://github.com/sameerulhaque)
- **LinkedIn**: [Sameer Ulhaq](https://linkedin.com/in/sameeruh97)
