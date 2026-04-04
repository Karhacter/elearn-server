using Bogus;
using elearn_server.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using elearn_server.Infrastructure.Persistence;

// seed 1000 users and 3 categories for testing purposes
public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public DbSeeder(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    private List<User> GenerateUsers(int count)
    {
        var faker = new Faker<User>()
            .RuleFor(u => u.FullName, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("09########"))
            .RuleFor(u => u.Role, f => f.PickRandom("Student", "Instructor"))
            .RuleFor(u => u.ProfilePicture, f => f.Internet.Avatar())
            .RuleFor(u => u.IsEmailVerified, true)
            .RuleFor(u => u.IsDeleted, true)
            .RuleFor(u => u.CreatedAt, DateTime.Now);

        var users = faker.Generate(count);

        // Hash password
        foreach (var user in users)
        {
            user.Password = _passwordHasher.HashPassword(user, "123456");
        }

        return users;
    }


    private List<Category> GenerateCategories()
    {
        return new List<Category>
        {
            new Category { Name = "Backend", Description = "Backend dev", CreatedAt = DateTime.Now },
            new Category { Name = "Frontend", Description = "Frontend dev", CreatedAt = DateTime.Now },
            new Category { Name = "Mobile", Description = "Mobile dev", CreatedAt = DateTime.Now }
        };
    }

    public void Seed()
    {
        if (_context.Users.Any()) return;

        var categories = GenerateCategories();
        _context.Categories.AddRange(categories);
        _context.SaveChanges();

        var users = GenerateUsers(1000);
        _context.Users.AddRange(users);
        _context.SaveChanges();

    }
}


