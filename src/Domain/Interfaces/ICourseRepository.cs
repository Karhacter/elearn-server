using elearn_server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Persistence.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<List<Course>> GetPagedAsync(int pageNumber, int pageSize);
    Task<int> CountAsync();
    Task<Course?> GetByIdAsync(int id);
    Task<List<Course>> GetByCategoryIdAsync(int categoryId);
    Task<List<Course>> SearchAsync(string? keyword, int? genreId, int? instructorId);
    Task<Course?> GetByTitleAsync(string title);
    Task<bool> UserExistsAsync(int userId);
    Task<bool> CategoryExistsAsync(int categoryId);
    Task AddAsync(Course course);
    void Remove(Course course);
    Task SaveChangesAsync();
    IQueryable<Course> RecommendationQuery();
}

