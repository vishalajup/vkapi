<<<<<<< HEAD
# vkapi
cicd testing
=======
# VK API - Sample Web API Project

A comprehensive .NET 9 Web API project with development-ready features including Swagger documentation, logging, validation, and CORS support.

## 🚀 Features

- **Swagger/OpenAPI Documentation** - Interactive API documentation with XML comments
- **Serilog Logging** - Structured logging with console and file outputs
- **FluentValidation** - Request validation with custom error messages
- **CORS Support** - Cross-origin resource sharing for frontend development
- **Health Checks** - Application health monitoring endpoints
- **JWT Bearer Authentication** - Ready for future authentication implementation
- **Comprehensive Error Handling** - Proper HTTP status codes and error responses

## 📋 Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- Git

## 🛠️ Setup Instructions

### 1. Clone and Navigate
```bash
git clone <repository-url>
cd vkapi
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Run the Application
```bash
dotnet run
```

The API will be available at:
- **API Base URL**: `https://localhost:7001` or `http://localhost:5001`
- **Swagger UI**: `https://localhost:7001/` (served at root)
- **Health Check**: `https://localhost:7001/health`

## 📚 API Endpoints

### Weather Forecast
- `GET /api/weatherforecast` - Get weather forecast (default 5 days)
- `GET /api/weatherforecast?days=7` - Get forecast for specific number of days
- `GET /api/weatherforecast/2024-01-15` - Get forecast for specific date
- `POST /api/weatherforecast` - Create new weather forecast

### System Endpoints
- `GET /health` - Application health check
- `GET /api/health` - Health status with timestamp
- `GET /api/info` - API information and version

## 🔧 Configuration

### Development Settings (`appsettings.Development.json`)
- Enhanced logging with debug level
- CORS enabled for common development ports
- Swagger UI available at root URL

### Production Settings (`appsettings.json`)
- Standard logging configuration
- CORS configuration for production origins
- File logging with daily rotation

## 🏗️ Project Structure

```
vkapi/
├── Controllers/
│   └── WeatherForecastController.cs    # Sample API controller
├── Program.cs                          # Application entry point
├── appsettings.json                    # Production configuration
├── appsettings.Development.json        # Development configuration
├── vkapi.csproj                       # Project file with dependencies
└── README.md                          # This file
```

## 🧪 Testing the API

### Using Swagger UI
1. Navigate to `https://localhost:7001/`
2. Explore available endpoints
3. Test API calls directly from the browser

### Using curl
```bash
# Get weather forecast
curl -X GET "https://localhost:7001/api/weatherforecast?days=3"

# Get health status
curl -X GET "https://localhost:7001/health"

# Create weather forecast
curl -X POST "https://localhost:7001/api/weatherforecast" \
  -H "Content-Type: application/json" \
  -d '{
    "date": "2024-01-15",
    "temperatureC": 25,
    "summary": "Warm"
  }'
```

## 📝 Logging

The application uses Serilog for structured logging:

- **Console Output**: Colored, structured logs in development
- **File Output**: Daily rotating log files in `logs/` directory
- **Log Levels**: Debug (development) / Information (production)

## 🔒 Security Features

- **Input Validation**: All requests validated using FluentValidation
- **CORS Configuration**: Properly configured for development and production
- **JWT Ready**: Bearer token authentication prepared for future implementation
- **HTTPS**: Enabled by default in development

## 🚀 Development Guidelines

### Adding New Controllers
1. Create controller in `Controllers/` directory
2. Use `[ApiController]` attribute
3. Add XML documentation comments
4. Include Swagger annotations
5. Add validation using FluentValidation

### Adding New DTOs
1. Use PascalCase for property names
2. Include XML documentation
3. Create separate request/response models
4. Add validation classes

### Logging Best Practices
```csharp
_logger.LogInformation("Processing request for {UserId}", userId);
_logger.LogWarning("Resource {ResourceId} not found", resourceId);
_logger.LogError(ex, "Error processing request");
```

## 🔄 Future Enhancements

- [ ] Database integration with Entity Framework
- [ ] Authentication and authorization
- [ ] Unit and integration tests
- [ ] Docker containerization
- [ ] CI/CD pipeline setup
- [ ] API versioning
- [ ] Rate limiting
- [ ] Caching implementation

## 📄 License

This project is for development and testing purposes.

## 🤝 Contributing

1. Follow the established coding conventions
2. Add proper documentation
3. Include validation for new endpoints
4. Test thoroughly before submitting

---

**Note**: This is a sample project for testing and development. Follow the SFA project guidelines for production implementations. 
>>>>>>> 35988f9 (Initial Commit)
