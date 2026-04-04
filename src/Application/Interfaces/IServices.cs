using System.Security.Claims;
using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Application.DTOs;
using elearn_server.Domain.Entities;

namespace elearn_server.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>> GetUsersAsync();
    Task<ServiceResult<AuthenticatedUserResponse>> GetUserByIdAsync(int id);
    Task<ServiceResult<LoginResponse>> LoginAsync(UserLoginDTO request, string? ipAddress, string? userAgent);
    Task<ServiceResult<AuthenticatedUserResponse>> RegisterAsync(RegisterUserDTO request, string verificationBaseUrl);
    Task<ServiceResult<LoginResponse>> RefreshTokenAsync(RefreshTokenRequestDTO request, string? ipAddress, string? userAgent);
    Task<ServiceResult<object>> VerifyEmailAsync(VerifyEmailRequestDTO request);
    Task<ServiceResult<object>> ForgotPasswordAsync(ForgotPasswordRequestDTO request);
    Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordRequestDTO request);
    Task<ServiceResult<object>> ChangePasswordAsync(int userId, ChangePasswordRequestDTO request, string? ipAddress, string? userAgent);
    Task<ServiceResult<AuthCheckResponse>> CheckAuthAsync(string token, ClaimsPrincipal user);
    Task<ServiceResult<object>> LogoutAsync(string token, string? ipAddress, string? userAgent);
}

public interface IUserService
{
    Task<ServiceResult<PagedResult<AuthenticatedUserResponse>>> GetAllAsync(int page, int pageSize);
    Task<ServiceResult<IReadOnlyCollection<AuthenticatedUserResponse>>> GetDeletedAsync();
    Task<ServiceResult<AuthenticatedUserResponse>> GetByIdAsync(int id);
    Task<ServiceResult<AuthenticatedUserResponse>> CreateAsync(UserCreateDTO request);
    Task<ServiceResult<AuthenticatedUserResponse>> UpdateAsync(int id, UserUpdateRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
    Task<ServiceResult<AuthenticatedUserResponse>> ToggleSoftDeleteAsync(int id);
    Task<ServiceResult<BulkSoftDeleteResponse>> BulkSoftDeleteAsync(BulkSoftDeleteRequest request);
    Task<ServiceResult<ImageUploadResponse>> UpdateImageAsync(int id, string imageUrl);
    Task<ServiceResult<ImageUploadResponse>> UploadImageAsync(int id, IFormFile imageFile, CancellationToken cancellationToken);
}

public interface ICategoryService
{
    Task<ServiceResult<IReadOnlyCollection<CategoryResponse>>> GetAllAsync();
    Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id);
    Task<ServiceResult<CategoryResponse>> CreateAsync(CategoryUpsertRequest request);
    Task<ServiceResult<CategoryResponse>> UpdateAsync(int id, CategoryUpsertRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}

public interface ICourseService
{
    Task<ServiceResult<PagedResult<CourseResponse>>> GetAllAsync(int page, int pageSize);
    Task<ServiceResult<PagedResult<CourseResponse>>> GetPagedAsync(int pageNumber, int pageSize);
    Task<ServiceResult<CourseResponse>> GetByIdAsync(int id);
    Task<ServiceResult<CourseResponse>> CreateAsync(CourseUpsertRequest request);
    Task<ServiceResult<CourseResponse>> UpdateAsync(int id, CourseUpsertRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
    Task<ServiceResult<CourseResponse>> ToggleSoftDeleteAsync(int id);
    Task<ServiceResult<BulkSoftDeleteResponse>> BulkSoftDeleteAsync(BulkSoftDeleteRequest request);
    Task<ServiceResult<IReadOnlyCollection<CourseResponse>>> GetByCategoryIdAsync(int categoryId);
    Task<ServiceResult<ImageUploadResponse>> UpdateImageAsync(int id, string imageUrl);
    Task<ServiceResult<ImageUploadResponse>> UploadImageAsync(int id, IFormFile imageFile, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyCollection<CourseResponse>>> SearchAsync(string? keyword, int? genreId, int? instructorId);
    Task<ServiceResult<CoursePreviewResponse>> PreviewAsync(int courseId);
    Task<ServiceResult<CourseResponse>> PublishAsync(int courseId, bool isAdmin);
    Task<ServiceResult<CourseResponse>> UnpublishAsync(int courseId);
    Task<ServiceResult<SectionResponse>> CreateSectionAsync(int courseId, SectionCreateRequest request);
    Task<ServiceResult<SectionResponse>> UpdateSectionAsync(int courseId, int sectionId, SectionUpdateRequest request);
    Task<ServiceResult<object>> DeleteSectionAsync(int courseId, int sectionId);
    Task<ServiceResult<IReadOnlyCollection<SectionResponse>>> ReorderSectionsAsync(int courseId, SectionReorderRequest request);
    Task<ServiceResult<PagedResult<LessonResponse>>> GetLessonsAsync(int courseId, int sectionId, int page, int pageSize);
    Task<ServiceResult<LessonResponse>> CreateLessonAsync(int courseId, int sectionId, LessonCreateRequest request);
    Task<ServiceResult<LessonResponse>> UpdateLessonAsync(int courseId, int sectionId, int lessonId, LessonUpdateRequest request);
    Task<ServiceResult<object>> DeleteLessonAsync(int courseId, int sectionId, int lessonId);
    Task<ServiceResult<LessonResponse>> ToggleLessonSoftDeleteAsync(int courseId, int sectionId, int lessonId);
    Task<ServiceResult<IReadOnlyCollection<LessonResponse>>> ReorderLessonsAsync(int courseId, int sectionId, LessonReorderRequest request);
}

public interface IProgressService
{
    Task<ServiceResult<IReadOnlyCollection<MyLearningItemResponse>>> GetMyLearningAsync(int userId);
    Task<ServiceResult<CourseProgressResponse>> GetCourseProgressAsync(int userId, int courseId);
    Task<ServiceResult<LessonProgressResponse>> CompleteLessonAsync(int userId, int lessonId);
    Task<ServiceResult<LessonProgressResponse>> UpdateResumePositionAsync(int userId, int lessonId, LessonResumePositionRequest request);
}

public interface IQuizService
{
    Task<ServiceResult<QuizResponse>> CreateQuizAsync(QuizUpsertRequest request);
    Task<ServiceResult<QuizResponse>> UpdateQuizAsync(int quizId, QuizUpsertRequest request);
    Task<ServiceResult<object>> DeleteQuizAsync(int quizId);
    Task<ServiceResult<QuizResponse>> CreateQuestionAsync(int quizId, QuizQuestionUpsertRequest request);
    Task<ServiceResult<QuizResponse>> UpdateQuestionAsync(int quizId, int questionId, QuizQuestionUpsertRequest request);
    Task<ServiceResult<object>> DeleteQuestionAsync(int quizId, int questionId);
    Task<ServiceResult<QuizAttemptStartResponse>> StartAttemptAsync(int quizId, int userId);
    Task<ServiceResult<QuizAttemptAnswerResponse>> SaveAnswerAsync(int attemptId, int userId, QuizAttemptAnswerRequest request);
    Task<ServiceResult<QuizAttemptResultResponse>> SubmitAttemptAsync(int attemptId, int userId);
    Task<ServiceResult<IReadOnlyCollection<QuizAttemptResultResponse>>> GetMyAttemptHistoryAsync(int quizId, int userId);
    Task<ServiceResult<QuizResultSummaryResponse>> GetMyResultSummaryAsync(int quizId, int userId);
}

public interface IAssignmentService
{
    Task<ServiceResult<AssignmentResponse>> CreateAssignmentAsync(AssignmentUpsertRequest request);
    Task<ServiceResult<AssignmentResponse>> UpdateAssignmentAsync(int assignmentId, AssignmentUpsertRequest request);
    Task<ServiceResult<object>> DeleteAssignmentAsync(int assignmentId);
    Task<ServiceResult<IReadOnlyCollection<AssignmentResponse>>> GetAssignmentsByCourseAsync(int courseId);
    Task<ServiceResult<AssignmentSubmissionResponse>> SubmitAssignmentAsync(int assignmentId, int studentId, AssignmentSubmissionRequest request, IFormFile? file, CancellationToken cancellationToken);
    Task<ServiceResult<AssignmentSubmissionResponse>> GetMySubmissionAsync(int assignmentId, int studentId);
    Task<ServiceResult<IReadOnlyCollection<AssignmentSubmissionResponse>>> GetSubmissionsAsync(int assignmentId);
    Task<ServiceResult<AssignmentSubmissionResponse>> GradeSubmissionAsync(int submissionId, int instructorId, AssignmentGradeRequest request);
}

public interface IOrderService
{
    Task<ServiceResult<IReadOnlyCollection<OrderResponse>>> GetAllAsync();
    Task<ServiceResult<OrderResponse>> GetByIdAsync(int id);
    Task<ServiceResult<OrderResponse>> CreateAsync(OrderRequest request);
    Task<ServiceResult<OrderResponse>> CreateBasicAsync(Order order);
    Task<ServiceResult<OrderResponse>> UpdateAsync(int id, OrderUpdateDTO request);
    Task<ServiceResult<object>> DeleteAsync(int id);
    Task<ServiceResult<IReadOnlyCollection<OrderResponse>>> GetByUserIdAsync(int userId);
}

public interface IOrderDetailService
{
    Task<ServiceResult<IReadOnlyCollection<OrderDetailResponse>>> GetAllAsync();
    Task<ServiceResult<OrderDetailResponse>> GetByIdAsync(int id);
    Task<ServiceResult<IReadOnlyCollection<OrderDetailResponse>>> GetByOrderIdAsync(int orderId);
    Task<ServiceResult<OrderDetailResponse>> CreateAsync(OrderDetailUpsertRequest request);
    Task<ServiceResult<OrderDetailResponse>> UpdateAsync(int id, OrderDetailUpsertRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}

public interface ICartService
{
    Task<ServiceResult<CartDTO>> GetCartByUserAsync(int userId, bool autoCreate);
    Task<ServiceResult<object>> InitCartsAsync();
    Task<ServiceResult<CartDTO>> AddToCartAsync(AddToCartRequest request);
    Task<ServiceResult<CartDTO>> UpdateCartItemAsync(UpdateCartItemRequest request);
    Task<ServiceResult<CartDTO>> RemoveItemAsync(RemoveCartItemRequest request);
    Task<ServiceResult<CartDTO>> ClearCartAsync(int userId);
}

public interface IWishlistService
{
    Task<ServiceResult<IReadOnlyCollection<WishlistResponse>>> GetByUserIdAsync(int userId);
    Task<ServiceResult<WishlistResponse>> CreateAsync(WishlistCreateRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}

public interface IPaymentService
{
    Task<ServiceResult<IReadOnlyCollection<PaymentResponse>>> GetByUserIdAsync(int userId);
    Task<ServiceResult<PaymentResponse>> CreateAsync(PaymentCreateRequest request);
    Task<ServiceResult<object>> ConfirmPaymentAsync(int orderId);
}

public interface IEnrollmentService
{
    Task<ServiceResult<IReadOnlyCollection<EnrollmentResponse>>> GetAllAsync();
    Task<ServiceResult<EnrollmentResponse>> CreateAsync(EnrollmentCreateRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
}

public interface ICommentService
{
    Task<ServiceResult<IReadOnlyCollection<CommentResponse>>> GetAllAsync();
    Task<ServiceResult<CommentResponse>> CreateAsync(CommentCreateRequest request);
    Task<ServiceResult<object>> DeleteAsync(int id);
    Task<ServiceResult<ReviewResponse>> CreateOrUpdateReviewAsync(int userId, int courseId, ReviewUpsertRequest request);
    Task<ServiceResult<IReadOnlyCollection<ReviewResponse>>> GetCourseReviewsAsync(int courseId, int? star, string? sortBy);
    Task<ServiceResult<CourseRatingSummaryResponse>> GetCourseRatingSummaryAsync(int courseId);
    Task<ServiceResult<ReviewResponse>> ReplyToReviewAsync(int ratingId, ReviewReplyRequest request);
    Task<ServiceResult<ReviewResponse>> ModerateReviewAsync(int ratingId, ReviewModerationRequest request);
}

public interface ICertificateService
{
    Task<ServiceResult<IReadOnlyCollection<CertificateResponse>>> GetAllAsync();
    Task<ServiceResult<CertificateEligibilityResponse>> CheckEligibilityAsync(int userId, int courseId);
    Task<ServiceResult<CertificateResponse>> GenerateCertificateAsync(CertificateGenerateRequest request, string verificationBaseUrl);
    Task<ServiceResult<CertificateVerificationResponse>> VerifyCertificateAsync(string code);
    Task<ServiceResult<CertificateResponse>> CreateAsync(CertificateCreateRequest request);
}

