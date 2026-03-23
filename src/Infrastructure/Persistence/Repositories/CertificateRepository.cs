using elearn_server.Infrastructure.Persistence;
using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using elearn_server.Domain.Interfaces;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public class CertificateRepository(AppDbContext context) : ICertificateRepository
{
    public Task<List<Certificate>> GetAllAsync() => context.Certificates.AsNoTracking().ToListAsync();
    public Task<User?> GetUserAsync(int userId) => context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
    public Task<Course?> GetCourseAsync(int courseId) => context.Courses.SingleOrDefaultAsync(c => c.CourseId == courseId);
    public Task AddAsync(Certificate certificate) => context.Certificates.AddAsync(certificate).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}

