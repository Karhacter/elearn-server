using Microsoft.EntityFrameworkCore;
using elearn_server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
<<<<<<< HEAD
=======
using elearn_server.Services;
>>>>>>> b9eee7b (wip: save today's work)

var builder = WebApplication.CreateBuilder(args);

// Configure JWT
var jwtConfig = builder.Configuration.GetSection("Jwt");
if (string.IsNullOrEmpty(jwtConfig["Key"]))
{
    throw new InvalidOperationException("JWT configuration is missing in appsettings.json");
}

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
<<<<<<< HEAD
=======
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection(OllamaOptions.SectionName));
builder.Services.AddHttpClient();
builder.Services.AddScoped<ICourseRecommendationService, CourseRecommendationService>();
>>>>>>> b9eee7b (wip: save today's work)

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
<<<<<<< HEAD
        policy.WithOrigins("http://localhost:3000") // Adjust this to your frontend URL
=======
        policy.WithOrigins("http://localhost:3000")
>>>>>>> b9eee7b (wip: save today's work)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

<<<<<<< HEAD
=======
// JWT
>>>>>>> b9eee7b (wip: save today's work)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]))
    };
});

<<<<<<< HEAD
// add the ignore
=======
>>>>>>> b9eee7b (wip: save today's work)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
<<<<<<< HEAD
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
=======

>>>>>>> b9eee7b (wip: save today's work)
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseStaticFiles();

<<<<<<< HEAD
// Use CORS middleware before authentication and authorization
app.UseCors("AllowFrontend");

// Enable static files middleware to serve files from wwwroot
app.UseStaticFiles();

=======
>>>>>>> b9eee7b (wip: save today's work)
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    await next.Invoke();
<<<<<<< HEAD
    if (context.Response.StatusCode == 403) // Forbidden
    {
        // Log the authorization failure
        Console.WriteLine("Authorization failed for user: " + context.User.Identity.Name);
=======
    if (context.Response.StatusCode == 403)
    {
        Console.WriteLine("Authorization failed for user: " + context.User.Identity?.Name);
>>>>>>> b9eee7b (wip: save today's work)
    }
});

app.MapControllers();

app.Run();
