
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;

namespace elearn_server.Application.Mappings;

public static class EntityMappings
{
    public static AuthenticatedUserResponse ToAuthenticatedUserResponse(this User user) => new()
    {
        UserId = user.UserId,
        FullName = user.FullName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Role = user.Role,
        ProfilePicture = user.ProfilePicture
    };

    public static CategoryResponse ToResponse(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description
    };

    public static CourseResponse ToResponse(this Course course) => new()
    {
        CourseId = course.CourseId,
        Title = course.Title,
        Description = course.Description,
        Slug = course.Slug,
        Status = course.Status.ToString(),
        IsSequential = course.IsSequential,
        Price = course.Price,
        Discount = course.Discount,
        GenreId = course.GenreId,
        GenreName = course.Genre?.Name,
        Image = course.Image,
        Thumbnail = course.Thumbnail,
        Duration = course.Duration,
        InstructorId = course.InstructorId,
        InstructorName = course.Instructor?.FullName,
        LearningOutcomes = course.LearningOutcomes?.OrderBy(o => o.Order).Select(o => o.Content).ToList() ?? new List<string>(),
        Requirements = course.Requirements?.OrderBy(r => r.Order).Select(r => r.Content).ToList() ?? new List<string>(),
        TargetAudiences = course.TargetAudiences?.OrderBy(t => t.Order).Select(t => t.Content).ToList() ?? new List<string>()
    };

    public static SectionResponse ToResponse(this CourseSection section) => new()
    {
        SectionId = section.SectionId,
        Title = section.Title,
        Description = section.Description,
        Order = section.Order,
        Lessons = section.Lessons?.OrderBy(l => l.Order).Select(ToResponse).ToList() ?? new List<LessonResponse>()
    };

    public static LessonResponse ToResponse(this Lesson lesson) => new()
    {
        LessonId = lesson.LessonId,
        Title = lesson.Title,
        ContentUrl = lesson.ContentUrl,
        Type = lesson.Type.ToString(),
        Duration = lesson.Duration,
        Order = lesson.Order,
        SectionId = lesson.SectionId
    };

    public static CoursePreviewResponse ToPreviewResponse(this Course course) => new()
    {
        CourseId = course.CourseId,
        Title = course.Title,
        Description = course.Description,
        Slug = course.Slug,
        Status = course.Status.ToString(),
        LearningOutcomes = course.LearningOutcomes?.OrderBy(o => o.Order).Select(o => o.Content).ToList() ?? new List<string>(),
        Requirements = course.Requirements?.OrderBy(r => r.Order).Select(r => r.Content).ToList() ?? new List<string>(),
        TargetAudiences = course.TargetAudiences?.OrderBy(t => t.Order).Select(t => t.Content).ToList() ?? new List<string>(),
        Sections = course.Sections?.OrderBy(s => s.Order).Select(ToResponse).ToList() ?? new List<SectionResponse>()
    };

    public static OrderDetailResponse ToResponse(this OrderDetail detail) => new()
    {
        Id = detail.Id,
        OrderId = detail.OrderId,
        CourseId = detail.CourseId,
        CourseTitle = detail.Course?.Title,
        Quantity = detail.Quantity,
        Price = detail.Price,
        Amount = detail.Amount
    };

    public static OrderResponse ToResponse(this Order order) => new()
    {
        OrderId = order.OrderID,
        Name = order.Name,
        Phone = order.Phone,
        Email = order.Email,
        Address = order.Address,
        UserId = order.user_id,
        Status = order.Status.ToString(),
        Items = order.OrderDetails?.Select(ToResponse).ToList() ?? new List<OrderDetailResponse>()
    };

    public static WishlistResponse ToResponse(this Wishlist wishlist) => new()
    {
        Id = wishlist.Id,
        UserId = wishlist.UserId,
        CourseId = wishlist.CourseId,
        CourseTitle = wishlist.Course?.Title,
        AddedDate = wishlist.AddedDate
    };

    public static PaymentResponse ToResponse(this Payment payment) => new()
    {
        Id = payment.Id,
        UserId = payment.UserId,
        CourseId = payment.CourseId,
        CourseTitle = payment.Course?.Title,
        Amount = payment.Amount,
        Method = payment.Method,
        PaymentDate = payment.PaymentDate
    };

    public static EnrollmentResponse ToResponse(this Enrollment enrollment) => new()
    {
        Id = enrollment.Id,
        UserId = enrollment.UserId,
        CourseId = enrollment.CourseId,
        CourseTitle = enrollment.Course?.Title,
        EnrollmentDate = enrollment.EnrollmentDate,
        IsActive = enrollment.IsActive
    };

    public static CommentResponse ToResponse(this Comment comment) => new()
    {
        Id = comment.Id,
        UserId = comment.UserId,
        CourseId = comment.CourseId,
        Content = comment.Content,
        CommentDate = comment.CommentDate
    };

    public static CertificateResponse ToResponse(this Certificate certificate) => new()
    {
        Id = certificate.Id,
        UserId = certificate.UserId,
        CourseId = certificate.CourseId,
        CertificateUrl = certificate.CertificateUrl,
        VerificationCode = certificate.VerificationCode,
        DateIssued = certificate.DateIssued
    };

}
