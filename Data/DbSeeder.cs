using Bogus;
using elearn_server.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using elearn_server.Infrastructure.Persistence;
using elearn_server.Domain.Enums;
using System.Text.Json;

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


    private List<Course> GenerateCourses(int count)
    {
        var instructors = _context.Users
            .Where(u => u.Role == "Instructor")
            .ToList();

        var categories = _context.Categories.ToList();

        var faker = new Faker<Course>()
            .RuleFor(c => c.Title, f => f.Lorem.Sentence(4, 2))
            .RuleFor(c => c.Description, f => f.Lorem.Paragraph(3))

            // slug chuẩn SEO
            .RuleFor(c => c.Slug, (f, c) => c.Title!.ToLower().Replace(" ", "-"))

            // status
            .RuleFor(c => c.Status, f => f.PickRandom<CourseStatus>())

            // học theo thứ tự hay không
            .RuleFor(c => c.IsSequential, f => f.Random.Bool())

            .RuleFor(c => c.Price, f => f.Random.Decimal(100000, 1000000))
            .RuleFor(c => c.Discount, f => f.Random.Int(0, 50))

            .RuleFor(c => c.Duration, f => f.Random.Int(120, 2000))

            .RuleFor(c => c.Image, f => f.Image.PicsumUrl())
            .RuleFor(c => c.Thumbnail, f => f.Image.PicsumUrl())

            .RuleFor(c => c.CreatedAt, DateTime.Now)

            // FK
            .RuleFor(c => c.GenreId, f => f.PickRandom(categories).Id)
            .RuleFor(c => c.InstructorId, f => f.PickRandom(instructors).UserId)

           // JSON fields
           .RuleFor(c => c.LearningOutcomes, (f, c) =>
            {
                return new List<LearningOutcome>
                {
                    new LearningOutcome { Content = f.Lorem.Sentence() },
                    new LearningOutcome { Content = f.Lorem.Sentence() },
                    new LearningOutcome { Content = f.Lorem.Sentence() }
                };
            })

           .RuleFor(c => c.Requirements, (f, c) =>
            {
                return new List<CourseRequirement>
                {
                    new CourseRequirement
                    {
                        Content = "Basic programming knowledge",
                        CourseId = c.CourseId
                    },
                    new CourseRequirement
                    {
                        Content = "Laptop or PC",
                        CourseId = c.CourseId
                    }
                };
            })

            .RuleFor(c => c.TargetAudiences, (f, c) =>
            {
                return new List<CourseTargetAudience>
                {
                    new CourseTargetAudience
                    {
                        Content = "Beginner developers",
                        CourseId = c.CourseId
                    },
                    new CourseTargetAudience
                    {
                        Content = "Students",
                        CourseId = c.CourseId
                    }
                };
            });

        return faker.Generate(count);
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
            .RuleFor(u => u.IsDeleted, false)
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
            new Category { Name = "Game", Description = "Game dev", CreatedAt = DateTime.Now },
            new Category { Name = "Mobile", Description = "Mobile dev", CreatedAt = DateTime.Now },
            new Category { Name = "AI", Description = "AI dev", CreatedAt = DateTime.Now },
            new Category { Name = "Data Science", Description = "Data Science dev", CreatedAt = DateTime.Now }
        };
    }
    private string GenerateSectionTitle(int order)
    {
        var titles = new[]
        {
        "Introduction",
        "Getting Started",
        "Core Concepts",
        "Advanced Topics",
        "Real Project",
        "Deployment"
    };

        return $"{order}. {titles[order - 1 < titles.Length ? order - 1 : 0]}";
    }

    public List<CourseSection> GenerateSections(List<Course> courses)
    {
        var faker = new Faker();
        var sections = new List<CourseSection>();

        foreach (var course in courses)
        {
            int sectionCount = faker.Random.Int(3, 6); // mỗi course có 3-6 section

            for (int i = 1; i <= sectionCount; i++)
            {
                sections.Add(new CourseSection
                {
                    Title = GenerateSectionTitle(i),
                    Description = faker.Lorem.Sentence(10),
                    Order = i,
                    CourseId = course.CourseId,
                    CreatedAt = DateTime.Now
                });
            }
        }

        return sections;
    }

    private string GenerateLessonTitle(string sectionTitle, int order)
    {
        var topics = new[]
        {
        "Introduction",
        "Setup Environment",
        "Core Concept",
        "Hands-on Practice",
        "Deep Dive",
        "Best Practices",
        "Summary"
    };

        return $"{order}. {topics[order - 1 < topics.Length ? order - 1 : 0]}";
    }

    private string GenerateContentUrl(LessonType type)
    {
        return type switch
        {
            LessonType.Video => "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            LessonType.Resource => "https://example.com/article",
            LessonType.Quiz => "https://example.com/quiz",
            _ => "https://example.com/content"
        };
    }

    private int GenerateDuration(LessonType type)
    {
        return type switch
        {
            LessonType.Video => Random.Shared.Next(5, 30),
            LessonType.Resource => Random.Shared.Next(3, 15),
            LessonType.Quiz => Random.Shared.Next(1, 10),
            _ => 5
        };
    }


    public List<Lesson> GenerateLessons(List<CourseSection> sections)
    {
        var faker = new Faker();
        var lessons = new List<Lesson>();

        var lessonTypes = new[]
        {
        LessonType.Video,
        LessonType.Resource,
        LessonType.Quiz
    };

        foreach (var section in sections)
        {
            int lessonCount = faker.Random.Int(4, 8);

            for (int i = 1; i <= lessonCount; i++)
            {
                var type = faker.PickRandom(lessonTypes);

                lessons.Add(new Lesson
                {
                    Title = GenerateLessonTitle(section.Title, i),
                    ContentUrl = GenerateContentUrl(type),
                    Type = type,
                    Duration = GenerateDuration(type),
                    Order = i,
                    SectionId = section.SectionId,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now
                });
            }
        }

        return lessons;
    }
    public void Seed()
    {
        if (_context.Users.Any()) return;

        var categories = GenerateCategories();
        _context.Categories.AddRange(categories);
        _context.SaveChanges();

        var users = GenerateUsers(30);
        _context.Users.AddRange(users);
        _context.SaveChanges();

        var courses = GenerateCourses(30);
        _context.Courses.AddRange(courses);
        _context.SaveChanges();

        var sections = GenerateSections(courses);
        _context.CourseSections.AddRange(sections);
        _context.SaveChanges();

        var lessons = GenerateLessons(sections);
        _context.Lessons.AddRange(lessons);
        _context.SaveChanges();
    }
}


