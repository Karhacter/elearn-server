using Microsoft.EntityFrameworkCore;
using elearn_server.Domain.Entities;

namespace elearn_server.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<CourseSection> CourseSections { get; set; }
    public DbSet<LearningOutcome> LearningOutcomes { get; set; }
    public DbSet<CourseRequirement> CourseRequirements { get; set; }
    public DbSet<CourseTargetAudience> CourseTargetAudiences { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }
    public DbSet<QuizAnswerOption> QuizAnswerOptions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<QuizAttemptAnswer> QuizAttemptAnswers { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<CourseProgress> CourseProgresses { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<LessonCompletion> LessonCompletions { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>().Property(c => c.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderDetail>().Property(od => od.Price).HasColumnType("decimal(18,2)");

        var sensitiveEntities = new[] { typeof(User), typeof(Course) };

        // Loop through all entities and foreign keys to set default delete behavior
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Exempt dependent entities that should be deleted when severed from Course
            var isDependentEntity = entity.ClrType == typeof(LearningOutcome) || 
                                   entity.ClrType == typeof(CourseRequirement) || 
                                   entity.ClrType == typeof(CourseTargetAudience);

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                // If the foreign key references a sensitive entity AND is not a dependent entity, set to NoAction
                if (sensitiveEntities.Contains(foreignKey.PrincipalEntityType.ClrType) && !isDependentEntity)
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
                }
            }
        }

        // Relationships and Constraints
        modelBuilder.Entity<Course>().ToTable("Course");

        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<User>().Property(u => u.Gender).HasConversion<string>();

        modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<CourseSection>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);

        modelBuilder.Entity<Course>().Property(c => c.Status).HasConversion<string>();
        modelBuilder.Entity<Course>().HasIndex(c => c.Slug).IsUnique();

        modelBuilder.Entity<Lesson>().Property(l => l.Type).HasConversion<string>();

        // User - Enrollment (One-to-Many)
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserId);

        // Course - Enrollment (One-to-Many)
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);

        // Course - Section (One-to-Many)
        modelBuilder.Entity<CourseSection>()
            .HasOne(s => s.Course)
            .WithMany(c => c.Sections)
            .HasForeignKey(s => s.CourseId);

        modelBuilder.Entity<CourseSection>()
            .HasIndex(s => new { s.CourseId, s.Order })
            .IsUnique();

        // Section - Lesson (One-to-Many)
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.CourseSection)
            .WithMany(s => s.Lessons)
            .HasForeignKey(l => l.SectionId);

        modelBuilder.Entity<Lesson>()
            .HasIndex(l => new { l.SectionId, l.Order })
            .IsUnique();

        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId);

        modelBuilder.Entity<LearningOutcome>()
            .HasOne(lo => lo.Course)
            .WithMany(c => c.LearningOutcomes)
            .HasForeignKey(lo => lo.CourseId);

        modelBuilder.Entity<CourseRequirement>()
            .HasOne(r => r.Course)
            .WithMany(c => c.Requirements)
            .HasForeignKey(r => r.CourseId);

        modelBuilder.Entity<CourseTargetAudience>()
            .HasOne(t => t.Course)
            .WithMany(c => c.TargetAudiences)
            .HasForeignKey(t => t.CourseId);

        modelBuilder.Entity<CourseProgress>()
            .HasIndex(cp => new { cp.UserId, cp.CourseId })
            .IsUnique();

        modelBuilder.Entity<CourseProgress>()
            .HasOne(cp => cp.User)
            .WithMany()
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CourseProgress>()
            .HasOne(cp => cp.Course)
            .WithMany()
            .HasForeignKey(cp => cp.CourseId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CourseProgress>()
            .HasOne(cp => cp.LastViewedLesson)
            .WithMany()
            .HasForeignKey(cp => cp.LastViewedLessonId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<LessonProgress>()
            .HasIndex(lp => new { lp.UserId, lp.LessonId })
            .IsUnique();

        modelBuilder.Entity<LessonProgress>()
            .HasOne(lp => lp.User)
            .WithMany()
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<LessonProgress>()
            .HasOne(lp => lp.Course)
            .WithMany()
            .HasForeignKey(lp => lp.CourseId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<LessonProgress>()
            .HasOne(lp => lp.Lesson)
            .WithMany()
            .HasForeignKey(lp => lp.LessonId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<LessonCompletion>()
            .HasIndex(lc => new { lc.UserId, lc.LessonId })
            .IsUnique();

        modelBuilder.Entity<LessonCompletion>()
            .HasOne(lc => lc.User)
            .WithMany()
            .HasForeignKey(lc => lc.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<LessonCompletion>()
            .HasOne(lc => lc.Course)
            .WithMany()
            .HasForeignKey(lc => lc.CourseId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<LessonCompletion>()
            .HasOne(lc => lc.Lesson)
            .WithMany()
            .HasForeignKey(lc => lc.LessonId)
            .OnDelete(DeleteBehavior.NoAction);

        // User - Rating (One-to-Many)
        modelBuilder.Entity<Rating>()
            .HasOne(r => r.User)
            .WithMany(u => u.Ratings)
            .HasForeignKey(r => r.UserId);

        // Course - Rating (One-to-Many)
        modelBuilder.Entity<Rating>()
            .HasOne(r => r.Course)
            .WithMany(c => c.Ratings)
            .HasForeignKey(r => r.CourseId);

        modelBuilder.Entity<Rating>()
            .Property(r => r.Status)
            .HasConversion<string>()
            .HasDefaultValue(Domain.Enums.ReviewStatus.Pending);

        modelBuilder.Entity<Rating>()
            .HasIndex(r => new { r.UserId, r.CourseId })
            .IsUnique();

        // Payment - User (One-to-Many)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Payment>()
            .Property(p => p.Status)
            .HasConversion<string>();

        // Payment - Course (One-to-Many)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Course)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CourseId);

        // Certificate - User (One-to-Many)
        modelBuilder.Entity<Certificate>()
            .HasOne(c => c.User)
            .WithMany(u => u.Certificates)
            .HasForeignKey(c => c.UserId);

        // Certificate - Course (One-to-Many)
        modelBuilder.Entity<Certificate>()
            .HasOne(c => c.Course)
            .WithMany(c => c.Certificates)
            .HasForeignKey(c => c.CourseId);

        modelBuilder.Entity<Certificate>()
            .HasIndex(c => new { c.UserId, c.CourseId })
            .IsUnique();

        modelBuilder.Entity<Certificate>()
            .HasIndex(c => c.VerificationCode)
            .IsUnique()
            .HasFilter("[VerificationCode] IS NOT NULL");

        // Quiz - Course (One-to-Many)
        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.Course)
            .WithMany(c => c.Quizzes)
            .HasForeignKey(q => q.CourseId);

        modelBuilder.Entity<QuizQuestion>()
            .Property(q => q.Type)
            .HasConversion<string>();

        modelBuilder.Entity<QuizAttempt>()
            .Property(a => a.Status)
            .HasConversion<string>();

        modelBuilder.Entity<QuizQuestion>()
            .HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId);

        modelBuilder.Entity<QuizQuestion>()
            .HasIndex(q => new { q.QuizId, q.Order })
            .IsUnique();

        modelBuilder.Entity<QuizAnswerOption>()
            .HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId);

        modelBuilder.Entity<QuizAnswerOption>()
            .HasIndex(o => new { o.QuestionId, o.Order })
            .IsUnique();

        modelBuilder.Entity<QuizAttempt>()
            .HasOne(a => a.Quiz)
            .WithMany(q => q.Attempts)
            .HasForeignKey(a => a.QuizId);

        modelBuilder.Entity<QuizAttempt>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<QuizAttempt>()
            .HasIndex(a => new { a.UserId, a.QuizId, a.AttemptNumber })
            .IsUnique();

        modelBuilder.Entity<QuizAttemptAnswer>()
            .HasOne(a => a.Attempt)
            .WithMany(at => at.Answers)
            .HasForeignKey(a => a.AttemptId);

        modelBuilder.Entity<QuizAttemptAnswer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.AttemptAnswers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<QuizAttemptAnswer>()
            .HasIndex(a => new { a.AttemptId, a.QuestionId })
            .IsUnique();

        // Assignment - Course (One-to-Many)
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Course)
            .WithMany(c => c.Assignments)
            .HasForeignKey(a => a.CourseId);

        modelBuilder.Entity<AssignmentSubmission>()
            .HasOne(s => s.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AssignmentId);

        modelBuilder.Entity<AssignmentSubmission>()
            .HasOne(s => s.Student)
            .WithMany()
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AssignmentSubmission>()
            .HasOne(s => s.GradedByInstructor)
            .WithMany()
            .HasForeignKey(s => s.GradedByInstructorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AssignmentSubmission>()
            .HasIndex(s => new { s.AssignmentId, s.StudentId })
            .IsUnique();

        // Comment - User (One-to-Many)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId);

        // Comment - Course (One-to-Many)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Course)
            .WithMany(c => c.Comments)
            .HasForeignKey(c => c.CourseId);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Rating)
            .WithOne(r => r.Comment)
            .HasForeignKey<Comment>(c => c.RatingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.RatingId)
            .IsUnique()
            .HasFilter("[RatingId] IS NOT NULL");

        // Notification - User (One-to-Many)
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId);

        // User - PasswordResetToken (One-to-Many)
        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(prt => prt.User)
            .WithMany()
            .HasForeignKey(prt => prt.UserId);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);

        modelBuilder.Entity<EmailVerificationToken>()
            .HasOne(evt => evt.User)
            .WithMany()
            .HasForeignKey(evt => evt.UserId);

        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Wishlist - Course (One-to-Many)
        modelBuilder.Entity<Wishlist>()
            .HasOne(w => w.Course)
            .WithMany(c => c.Wishlists)
            .HasForeignKey(w => w.CourseId);

        // Email of User is unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // A User can only enroll in a Course ONCE
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique();

        // Seed Data
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, RoleName = "Admin" },
            new Role { Id = 2, RoleName = "Instructor" },
            new Role { Id = 3, RoleName = "Student" }
        );
    }
}
