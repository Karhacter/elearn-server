using elearn_server.Application.Interfaces;
using elearn_server.Common.Options;
using elearn_server.Domain.Interfaces;
using elearn_server.Infrastructure.Persistence;
using elearn_server.Infrastructure.Persistence.Repositories;
using elearn_server.Infrastructure.Services;
using elearn_server.Infrastructure.Services.Core.Auth;
using elearn_server.Infrastructure.Services.Core.Assignments;
using elearn_server.Infrastructure.Services.Core.Categories;
using elearn_server.Infrastructure.Services.Core.Courses;
using elearn_server.Infrastructure.Services.Core.Notifications;
using elearn_server.Infrastructure.Services.Core.Progress;
using elearn_server.Infrastructure.Services.Core.Quizzes;
using elearn_server.Infrastructure.Services.Core.Users;
using elearn_server.Infrastructure.Services.Commerce;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<AuthSecurityOptions>(configuration.GetSection(AuthSecurityOptions.SectionName));
        services.Configure<OllamaOptions>(configuration.GetSection(OllamaOptions.SectionName));
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Infrastructure services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddHttpClient();
        services.AddScoped<ICourseRecommendationService, CourseRecommendationService>();

        // Core services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<IQuizService, QuizService>();

        // Commerce services
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderDetailService, OrderDetailService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<IPostService, PostService>();
        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<Program>();
        return services;
    }
}
