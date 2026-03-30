using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities;

public class CourseProgress : BaseEntity
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public double ProgressPercent { get; set; }

    public int? LastViewedLessonId { get; set; }
    public Lesson? LastViewedLesson { get; set; }
    public DateTime? LastViewedAt { get; set; }
}
