using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

// POST /settings  1.1
// Content-Type: application/json
// Host: localhost:10000
// Content-Length: 138
//
// {
//   "Endpoint": "https://www.github.com",
// }

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(
    "v1",
    options =>
    {
        options.AddSchemaTransformer(
            (schema, context, _) =>
            {
                if (context.JsonTypeInfo.Type == typeof(Settings))
                {
                    schema.Properties.Clear();
                    schema.Properties.Add(
                        "Endpoint",
                        new OpenApiSchema
                        {
                            Type = "string",
                            Example = new OpenApiString("https://www.github.com"),
                            Description = "The URL or endpoint to which the request or message will be sent."
                        }
                    );

                    // Add other properties
                }

                return Task.CompletedTask;
            }
        );
    }
);

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapPost("/settings", async (HttpRequest request) =>
{
    // accepts other properties dynamically
    using var reader = new StreamReader(request.Body);
    var requestBody = await reader.ReadToEndAsync();
    var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

    return Results.Json(settings);
}).Accepts<Settings>("application/json");

app.MapGet("/", () => Results.Redirect("/scalar/v1"))
    .ExcludeFromDescription();

app.Run();

public record Settings;
