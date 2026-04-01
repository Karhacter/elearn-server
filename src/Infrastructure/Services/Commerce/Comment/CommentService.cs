using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Domain.Enums;
using elearn_server.Infrastructure.Persistence.Repositories;

namespace elearn_server.Infrastructure.Services.Commerce;

public class CommentService(ICommentRepository repository) : ICommentService
{
    public async Task<ServiceResult<IReadOnlyCollection<CommentResponse>>> GetAllAsync() =>
        ServiceResult<IReadOnlyCollection<CommentResponse>>.Ok((await repository.GetAllAsync()).Select(c => c.ToResponse()).ToList());

    public async Task<ServiceResult<CommentResponse>> CreateAsync(CommentCreateRequest request)
    {
        if (await repository.GetUserAsync(request.UserId) is null)
        {
            return ServiceResult<CommentResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        if (await repository.GetCourseAsync(request.CourseId) is null)
        {
            return ServiceResult<CommentResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var comment = new Comment
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            Content = request.Content.Trim(),
            CommentDate = DateTime.UtcNow
        };

        await repository.AddAsync(comment);
        await repository.SaveChangesAsync();
        return ServiceResult<CommentResponse>.Created(comment.ToResponse(), "Comment created successfully.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var comment = await repository.GetByIdAsync(id);
        if (comment is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Comment not found.");
        }

        repository.Remove(comment);
        await repository.SaveChangesAsync();
        return ServiceResult<object>.Ok(null, "Comment deleted successfully.");
    }

    public async Task<ServiceResult<ReviewResponse>> CreateOrUpdateReviewAsync(int userId, int courseId, ReviewUpsertRequest request)
    {
        if (request.Score is < 1 or > 5)
        {
            return ServiceResult<ReviewResponse>.Fail(StatusCodes.Status400BadRequest, "Rating score must be between 1 and 5.");
        }

        if (await repository.GetUserAsync(userId) is null)
        {
            return ServiceResult<ReviewResponse>.Fail(StatusCodes.Status404NotFound, "User not found.");
        }

        if (await repository.GetCourseAsync(courseId) is null)
        {
            return ServiceResult<ReviewResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        if (!await repository.IsUserEnrolledAsync(userId, courseId))
        {
            return ServiceResult<ReviewResponse>.Fail(StatusCodes.Status403Forbidden, "You can only review courses you are enrolled in.");
        }

        var rating = await repository.GetRatingByUserAndCourseAsync(userId, courseId);
        if (rating is null)
        {
            rating = new Rating
            {
                UserId = userId,
                CourseId = courseId,
                Score = request.Score,
                Status = ReviewStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await repository.AddRatingAsync(rating);
            await repository.SaveChangesAsync();
        }
        else
        {
            rating.Score = request.Score;
            rating.Status = ReviewStatus.Pending;
            rating.ReplyContent = null;
            rating.ReplyTimestamp = null;
        }

        var commentText = request.Comment?.Trim();
        var reviewComment = await repository.GetReviewCommentByRatingIdAsync(rating.RatingId)
                           ?? await repository.GetReviewCommentByUserAndCourseAsync(userId, courseId);

        if (string.IsNullOrWhiteSpace(commentText))
        {
            if (reviewComment is not null && reviewComment.RatingId == rating.RatingId)
            {
                repository.Remove(reviewComment);
            }
        }
        else if (reviewComment is null)
        {
            await repository.AddAsync(new Comment
            {
                UserId = userId,
                CourseId = courseId,
                RatingId = rating.RatingId,
                Content = commentText,
                CommentDate = DateTime.UtcNow
            });
        }
        else
        {
            reviewComment.Content = commentText;
            reviewComment.CommentDate = DateTime.UtcNow;
            reviewComment.RatingId = rating.RatingId;
        }

        await repository.SaveChangesAsync();

        var finalComment = await repository.GetReviewCommentByRatingIdAsync(rating.RatingId);
        return ServiceResult<ReviewResponse>.Ok(ToReviewResponse(rating, finalComment), "Review saved successfully.");
    }

    public async Task<ServiceResult<IReadOnlyCollection<ReviewResponse>>> GetCourseReviewsAsync(int courseId, int? star, string? sortBy)
    {
        if (star.HasValue && (star.Value < 1 || star.Value > 5))
        {
            return ServiceResult<IReadOnlyCollection<ReviewResponse>>.Fail(StatusCodes.Status400BadRequest, "Star filter must be between 1 and 5.");
        }

        if (await repository.GetCourseAsync(courseId) is null)
        {
            return ServiceResult<IReadOnlyCollection<ReviewResponse>>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var normalizedSort = string.IsNullOrWhiteSpace(sortBy) ? "newest" : sortBy.Trim().ToLowerInvariant();
        if (normalizedSort is not ("newest" or "highest"))
        {
            return ServiceResult<IReadOnlyCollection<ReviewResponse>>.Fail(StatusCodes.Status400BadRequest, "sortBy must be 'newest' or 'highest'.");
        }

        var ratings = await repository.GetApprovedRatingsByCourseAsync(courseId, star, normalizedSort);
        var responses = ratings.Select(r => ToReviewResponse(r, r.Comment)).ToList();
        return ServiceResult<IReadOnlyCollection<ReviewResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<CourseRatingSummaryResponse>> GetCourseRatingSummaryAsync(int courseId)
    {
        if (await repository.GetCourseAsync(courseId) is null)
        {
            return ServiceResult<CourseRatingSummaryResponse>.Fail(StatusCodes.Status404NotFound, "Course not found.");
        }

        var ratings = await repository.GetApprovedRatingsByCourseAsync(courseId);
        if (ratings.Count == 0)
        {
            return ServiceResult<CourseRatingSummaryResponse>.Ok(new CourseRatingSummaryResponse
            {
                CourseId = courseId
            });
        }

        var summary = new CourseRatingSummaryResponse
        {
            CourseId = courseId,
            AverageRating = Math.Round(ratings.Average(r => r.Score), 2),
            TotalReviews = ratings.Count,
            OneStarCount = ratings.Count(r => r.Score == 1),
            TwoStarCount = ratings.Count(r => r.Score == 2),
            ThreeStarCount = ratings.Count(r => r.Score == 3),
            FourStarCount = ratings.Count(r => r.Score == 4),
            FiveStarCount = ratings.Count(r => r.Score == 5)
        };

        return ServiceResult<CourseRatingSummaryResponse>.Ok(summary);
    }

    public async Task<ServiceResult<ReviewResponse>> ReplyToReviewAsync(int ratingId, ReviewReplyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ReplyContent))
        {
            return ServiceResult<ReviewResponse>.Fail(StatusCodes.Status400BadRequest, "Reply content is required.");
        }

        var rating = await repository.GetRatingByIdAsync(ratingId);
        if (rating is null)
        {
            return ServiceResult<ReviewResponse>.Fail(StatusCodes.Status404NotFound, "Review not found.");
        }

        rating.ReplyContent = request.ReplyContent.Trim();
        rating.ReplyTimestamp = DateTime.UtcNow;
        await repository.SaveChangesAsync();

        var reviewComment = await repository.GetReviewCommentByRatingIdAsync(ratingId);
        return ServiceResult<ReviewResponse>.Ok(ToReviewResponse(rating, reviewComment), "Reply added successfully.");
    }

    public async Task<ServiceResult<ReviewResponse>> ModerateReviewAsync(int ratingId, ReviewModerationRequest request)
    {
        var rating = await repository.GetRatingByIdAsync(ratingId);
        if (rating is null)
        {
            return ServiceResult<ReviewResponse>.Fail(StatusCodes.Status404NotFound, "Review not found.");
        }

        rating.Status = request.Status;
        await repository.SaveChangesAsync();

        var reviewComment = await repository.GetReviewCommentByRatingIdAsync(ratingId);
        return ServiceResult<ReviewResponse>.Ok(ToReviewResponse(rating, reviewComment), "Review status updated successfully.");
    }

    private static ReviewResponse ToReviewResponse(Rating rating, Comment? comment) => new()
    {
        RatingId = rating.RatingId,
        UserId = rating.UserId,
        UserName = rating.User?.FullName,
        CourseId = rating.CourseId,
        Score = rating.Score,
        Comment = comment?.Content,
        Status = rating.Status.ToString(),
        ReplyContent = rating.ReplyContent,
        ReplyTimestamp = rating.ReplyTimestamp,
        CreatedAt = rating.CreatedAt
    };
}
