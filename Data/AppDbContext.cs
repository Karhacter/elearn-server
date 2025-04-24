using Microsoft.EntityFrameworkCore;
using elearn_server.Models;

namespace elearn_server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Course> todoItems { get; set; } = null!;
    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
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
    public DbSet<OrderDetail> orderDetails { get; set; }
    public DbSet<StatusOrder> StatusOrders { get; set; }
    public DbSet<Cart> Carts { get; set; }

    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<elearn_server.Models.BlacklistedToken> BlacklistedTokens { get; set; }

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

        // Course - Lesson (One-to-Many)
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId);

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

        // Wishlist - Course (One-to-Many)
        modelBuilder.Entity<Wishlist>()
            .HasOne(w => w.Course)
            .WithMany(c => c.Wishlists)
            .HasForeignKey(w => w.CourseId);
    }
}


