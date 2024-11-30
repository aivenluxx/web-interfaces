using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ������ Swagger ����� ��� �������� Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer(); // ������ ��������� ��� API
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Web API", Version = "v1" }); // ������������� OpenApiInfo

        // ������ ������������ ��� �������������� �� ��������� Bearer ������
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Please enter your token in the format: Bearer {your token}"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });
}

var app = builder.Build();

// ������������ ��� Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // �������� Swagger ��� ����������� ������������
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web API v1"); // ����������� Swagger UI � ���������� ���������
        c.RoutePrefix = string.Empty; // ������ Swagger UI �� �����
    });
}

app.UseHttpsRedirection();

// ����� ����� ��� �������� ������
var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

// ����-��� ��� �������� ������ �� �����
var cityWeatherData = new Dictionary<string, (int MinTemp, int MaxTemp, string Summary)>
{
    { "Kyiv", (-5, 10, "Chilly") },
    { "Lviv", (-2, 8, "Cool") },
    { "Odesa", (5, 15, "Mild") },
    { "Kharkiv", (-7, 4, "Freezing") },
    { "Dnipro", (0, 12, "Warm") }
};

// ����������� ������� ��� �������� ������ � ���������� city (GET)
app.MapGet("/weatherforecast", (HttpContext context) =>
{
    var authorizationHeader = context.Request.Headers["Authorization"].ToString();
    if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
    {
        return Results.Unauthorized(); // ������ ��������� 401 ��� �����������
    }

    var city = context.Request.Query["city"].ToString();

    if (string.IsNullOrEmpty(city) || !cityWeatherData.ContainsKey(city))
    {
        return Results.NotFound(new { message = $"City '{city}' not found" });
    }

    var cityForecast = cityWeatherData[city];
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(cityForecast.MinTemp, cityForecast.MaxTemp),
            cityForecast.Summary
        ))
        .ToArray();

    return Results.Ok(forecast); // ��������� ��� � ������ ���������� Ok
})
.WithName("GetWeatherForecast")
.WithOpenApi(); // ������ OpenAPI ������������ ��� ����� ��������

// ������ POST ����� ��� ��������� ������ �������� ������
app.MapPost("/weatherforecast", (WeatherForecast newForecast) =>
{
    // ����� ��� ������� ����� ��������
    if (newForecast == null)
    {
        return Results.BadRequest(new { message = "Invalid forecast data" });
    }

    // ������ ������� �� ���� "���� �����"
    var city = newForecast.Summary; // �� ���� �������, ��� ����� �������� �� �������� ��� ���� �����
    cityWeatherData[city] = (newForecast.TemperatureC, newForecast.TemperatureC + 10, newForecast.Summary);

    return Results.Created($"/weatherforecast/{city}", newForecast); // ��������� ������ 201 � ����� ���������
})
.WithName("CreateWeatherForecast")
.WithOpenApi(); // ������ OpenAPI ������������ ��� ����� ��������

app.Run();

// ��������� ������ WeatherForecast ������
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556); // ������������ ����������� � ������� ����������
}
