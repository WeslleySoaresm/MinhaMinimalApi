# About this Project

This is a .NET 9 Minimal API project. The goal is to create a simple and lightweight web API.

## Getting Started

The project is configured to run with the .NET CLI.

To build and run the application, use the following command from the root directory:
    
```sh
dotnet run
```

The API will be available at the URLs specified in `Properties/launchSettings.json`, typically `http://localhost:5000` or `https://localhost:5001`.

## Project Structure

- `Program.cs`: This is the main entry point of the application. It contains the application setup, service registration, and endpoint definitions. As a Minimal API, all routes are defined here.
- `MinhaMinimalApi.csproj`: The C# project file, which defines project properties, target framework (`net9.0`), and package dependencies.
- `appsettings.json`: Configuration file for the application.

## Key Dependencies

- **Microsoft.AspNetCore.OpenApi**: Used to generate OpenAPI (Swagger) documentation for the API. This is configured in `Program.cs`.

## How to Add New Endpoints

Endpoints are mapped directly in `Program.cs` using the `app.Map...` methods. Follow the existing pattern to add new endpoints.

For example, to add a new GET endpoint for `/products`:

```csharp
// In Program.cs

// ... existing code
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Add new endpoint here
app.MapGet("/products", () => {
    // Your logic to fetch products
    return new [] { "Product 1", "Product 2" };
});

app.Run();
```
