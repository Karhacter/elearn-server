namespace elearn_server.Extensions;

public static class CorsExtensions
{
    public const string FrontendPolicy = "AllowFrontend";

    public static IServiceCollection AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        var frontendUrl = configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendPolicy, policy =>
            {
                policy.WithOrigins(frontendUrl)
                      .AllowCredentials()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }
}
