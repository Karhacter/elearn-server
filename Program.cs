using elearn_server.Extensions;
using elearn_server.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Validate JWT config early
if (string.IsNullOrEmpty(builder.Configuration.GetSection("Jwt")["Key"]))
    throw new InvalidOperationException("JWT configuration is missing in appsettings.json");

// Logging
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Register services via extension methods
builder.Services
    .AddDatabase(builder.Configuration)
    .AddOptions(builder.Configuration)
    .AddRepositories()
    .AddApplicationServices()
    .AddValidation()
    .AddJwtAuthentication(builder.Configuration)
    .AddFrontendCors(builder.Configuration)
    .AddApiRateLimiter()
    .AddAuthorization();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                e => e.Key,
                e => e.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
            );

        return new BadRequestObjectResult(new
        {
            Success = false,
            Message = "Validation failed.",
            Errors = errors
        });
    };
});

builder.Services.AddOpenApi();

// ─── Build App ───

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options => options.DocumentPath = "/openapi/v1.json");
}

app.UseHttpsRedirection();
app.UseCors(CorsExtensions.FrontendPolicy);
app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    await next.Invoke();
    if (context.Response.StatusCode == 403)
        Console.WriteLine("Authorization failed for user: " + context.User.Identity?.Name);
});

app.MapControllers();
app.Run();
