
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Infrastructure.Persistence.Repositories.IRepository;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository(AppDbContext context) : IEnrollmentRepository
{
    public Task<List<Enrollment>> GetAllAsync() => context.Enrollments.Include(e => e.Course).AsNoTracking().ToListAsync();
    public Task<Enrollment?> GetByIdAsync(int id) => context.Enrollments.SingleOrDefaultAsync(e => e.Id == id);
    public Task<Enrollment?> GetByUserAndCourseAsync(int userId, int courseId) => context.Enrollments.Include(e => e.Course).SingleOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
    public Task<User?> GetUserAsync(int userId) => context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
    public Task<Course?> GetCourseAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);
    public Task AddAsync(Enrollment enrollment) => context.Enrollments.AddAsync(enrollment).AsTask();
    public void Remove(Enrollment enrollment) => context.Enrollments.Remove(enrollment);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

