namespace elearn_server.Extensions;

public static class CorsExtensions
{
    public const string FrontendPolicy = "AllowFrontend";

    public static IServiceCollection AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        var frontendUrls = configuration["App:FrontendBaseUrl"]?.Split(",") ?? new[] { "http://localhost:4200", "http://localhost:64934" };

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendPolicy, policy =>
            {
                policy.WithOrigins(frontendUrls)
                      .AllowCredentials()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }
}
