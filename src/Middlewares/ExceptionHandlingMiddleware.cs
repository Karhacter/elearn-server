using System.Net;
using System.Text.Json;

namespace elearn_server.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Đi tiếp vào Controller
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi hệ thống xảy ra: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Cấu trúc JSON trả về thống nhất
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Đã xảy ra lỗi từ phía máy chủ. Vui lòng thử lại sau.",
            Detailed = exception.Message // Có thể ẩn dòng này ở môi trường Production
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}