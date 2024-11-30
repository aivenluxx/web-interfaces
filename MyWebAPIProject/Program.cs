using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Додаємо Swagger тільки для оточення Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer(); // Додаємо експлорер для API
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Web API", Version = "v1" }); // Використовуємо OpenApiInfo

        // Додаємо налаштування для аутентифікації за допомогою Bearer токену
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

// Налаштування для Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Включаємо Swagger для генерування документації
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web API v1"); // Налаштовуємо Swagger UI з правильним ендпоінтом
        c.RoutePrefix = string.Empty; // Додаємо Swagger UI на корінь
    });
}

app.UseHttpsRedirection();

// Масив описів для прогнозу погоди
var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

// Демо-дані для прогнозу погоди по містах
var cityWeatherData = new Dictionary<string, (int MinTemp, int MaxTemp, string Summary)>
{
    { "Kyiv", (-5, 10, "Chilly") },
    { "Lviv", (-2, 8, "Cool") },
    { "Odesa", (5, 15, "Mild") },
    { "Kharkiv", (-7, 4, "Freezing") },
    { "Dnipro", (0, 12, "Warm") }
};

// Налаштовуємо маршрут для прогнозу погоди з параметром city (GET)
app.MapGet("/weatherforecast", (HttpContext context) =>
{
    var authorizationHeader = context.Request.Headers["Authorization"].ToString();
    if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
    {
        return Results.Unauthorized(); // Просто повертаємо 401 без повідомлення
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

    return Results.Ok(forecast); // Повертаємо дані у вигляді результату Ok
})
.WithName("GetWeatherForecast")
.WithOpenApi(); // Додаємо OpenAPI документацію для цього ендпоінту

// Додаємо POST запит для створення нового прогнозу погоди
app.MapPost("/weatherforecast", (WeatherForecast newForecast) =>
{
    // Логіка для обробки нових прогнозів
    if (newForecast == null)
    {
        return Results.BadRequest(new { message = "Invalid forecast data" });
    }

    // Додамо прогноз до нашої "бази даних"
    var city = newForecast.Summary; // Це лише приклад, тут можна додавати до словника або бази даних
    cityWeatherData[city] = (newForecast.TemperatureC, newForecast.TemperatureC + 10, newForecast.Summary);

    return Results.Created($"/weatherforecast/{city}", newForecast); // Повертаємо статус 201 з новим прогнозом
})
.WithName("CreateWeatherForecast")
.WithOpenApi(); // Додаємо OpenAPI документацію для цього ендпоінту

app.Run();

// Оголошуємо модель WeatherForecast вручну
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556); // Перетворення температури в градуси Фаренгейта
}
