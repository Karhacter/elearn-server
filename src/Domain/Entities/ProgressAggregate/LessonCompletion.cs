using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities;

public class LessonCompletion : BaseEntity
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }

    public DateTime CompletedAt { get; set; }
}
