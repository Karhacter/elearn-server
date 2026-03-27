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
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

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



        // Loop through all entities and foreign keys
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                // If the foreign key references a sensitive entity, set to NoAction
                if (sensitiveEntities.Contains(foreignKey.PrincipalEntityType.ClrType))
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
                }
            }
        }
        // Relationships and Constraints
        modelBuilder.Entity<Course>()
                       .Property(c => c.Status)
                       .HasConversion<string>();

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Slug)
            .IsUnique();

        modelBuilder.Entity<Lesson>()
            .Property(l => l.Type)
            .HasConversion<string>();

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

        // Payment - User (One-to-Many)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<Order>()
                    .Property(o => o.Status)
                    .HasConversion<string>(); // "Pending", "Completed"...

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

        // Quiz - Course (One-to-Many)
        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.Course)
            .WithMany(c => c.Quizzes)
            .HasForeignKey(q => q.CourseId);

        // Assignment - Course (One-to-Many)
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Course)
            .WithMany(c => c.Assignments)
            .HasForeignKey(a => a.CourseId);

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
            .OnDelete(DeleteBehavior.NoAction);

        // Wishlist - Course (One-to-Many)
        modelBuilder.Entity<Wishlist>()
            .HasOne(w => w.Course)
            .WithMany(c => c.Wishlists)
            .HasForeignKey(w => w.CourseId);

        // Email của User là duy nhất
        modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

        // Một User chỉ được Enrollment (ghi danh) vào một Course MỘT LẦN DUY NHẤT
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


