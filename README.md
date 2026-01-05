## 🔹 Logging Integration

This project uses **ASP.NET Core built-in logging (`Microsoft.Extensions.Logging`)**.

### Why this logging approach was chosen
- It is built into ASP.NET Core
- No external library is required
- Fully integrated with Dependency Injection
- Lightweight and suitable for REST APIs
- Can be easily extended with Serilog or other providers in the future

### How logging is implemented
- A global exception middleware (`GlobalExceptionMiddleware`) is used
- All unhandled exceptions are caught in one place
- Errors are logged using `ILogger<GlobalExceptionMiddleware>`
- Logged data includes:
  - Exception details
  - HTTP request path
  - HTTP request method

### How to view logs
- Logs are written to the **console**
- Run the application with:
  ```bash
  dotnet run
