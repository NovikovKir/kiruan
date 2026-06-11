using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class SeedData
{
    public static async Task EnsureSeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        bool adminExists = false;
        bool userExists = false;
        bool moviesExists = false;

        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = configuration["AdminSettings:Email"] ?? "admin@example.com";
        var adminPassword = configuration["AdminSettings:Password"] ?? "Admin123!";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin != null)
        {
            adminExists = true;
        }

        if (!adminExists)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                Role = "Admin",
                SubscriptionStatus = "Active",
                SubscriptionEndAt = DateTime.UtcNow.AddYears(100)
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        var existingUser = await userManager.FindByEmailAsync("user@example.com");
        if (existingUser != null)
        {
            userExists = true;
        }

        if (!userExists)
        {
            var userUser = new ApplicationUser
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                EmailConfirmed = true,
                Role = "User",
                SubscriptionStatus = "Inactive"
            };

            var createResult = await userManager.CreateAsync(userUser, "userPassword");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(userUser, "User");
            }
        }

        if (await db.Movies.AnyAsync())
        {
            moviesExists = true;
        }

        if (!moviesExists)
        {
            db.Movies.AddRange(
                new Movie
                {
                    Title = "Матрица",
                    Description = "Хакер Нео узнаёт, что реальный мир — симуляция, и присоединяется к восстанию против машин.",
                    Year = 1999,
                    DurationMinutes = 136,
                    AgeRating = "16+",
                    FilePath = "/var/opt/mssql-files/films/matrix.mp4",
                    AudioTracks =
                    [
                        new AudioTrack { Language = "Русская", FilePath = "/var/opt/mssql-files/audio/matrix_ru.mp3" },
                        new AudioTrack { Language = "Оригинал (англ.)", FilePath = "/var/opt/mssql-files/audio/matrix_en.mp3" }
                    ],
                    Subtitles =
                    [
                        new Subtitle { Language = "Русские", FilePath = "/var/opt/mssql-files/subtitles/matrix_ru.vtt" },
                        new Subtitle { Language = "Английские", FilePath = "/var/opt/mssql-files/subtitles/matrix_en.vtt" }
                    ]
                },
                new Movie
                {
                    Title = "Начало",
                    Description = "Специалист по промышленному шпионажу проникает в сны людей, чтобы украсть идеи и изменить реальность.",
                    Year = 2010,
                    DurationMinutes = 148,
                    AgeRating = "12+",
                    FilePath = "/var/opt/mssql-files/films/inception.mp4",
                    AudioTracks =
                    [
                        new AudioTrack { Language = "Русская", FilePath = "/var/opt/mssql-files/audio/inception_ru.mp3" },
                        new AudioTrack { Language = "Оригинал (англ.)", FilePath = "/var/opt/mssql-files/audio/inception_en.mp3" }
                    ],
                    Subtitles =
                    [
                        new Subtitle { Language = "Русские", FilePath = "/var/opt/mssql-files/subtitles/inception_ru.vtt" },
                        new Subtitle { Language = "Английские", FilePath = "/var/opt/mssql-files/subtitles/inception_en.vtt" }
                    ]
                },
                new Movie
                {
                    Title = "Титаник",
                    Description = "История любви бедного художника и аристократки на борту легендарного лайнера.",
                    Year = 1997,
                    DurationMinutes = 194,
                    AgeRating = "12+",
                    FilePath = "/var/opt/mssql-files/films/titanic.mp4",
                    AudioTracks =
                    [
                        new AudioTrack { Language = "Русская", FilePath = "/var/opt/mssql-files/audio/titanic_ru.mp3" },
                        new AudioTrack { Language = "Оригинал (англ.)", FilePath = "/var/opt/mssql-files/audio/titanic_en.mp3" }
                    ],
                    Subtitles =
                    [
                        new Subtitle { Language = "Русские", FilePath = "/var/opt/mssql-files/subtitles/titanic_ru.vtt" },
                        new Subtitle { Language = "Английские", FilePath = "/var/opt/mssql-files/subtitles/titanic_en.vtt" }
                    ]
                },
                new Movie
                {
                    Title = "Гарри Поттер и философский камень",
                    Description = "Юный волшебник узнаёт о своём происхождении и начинает обучение в Хогвартсе.",
                    Year = 2001,
                    DurationMinutes = 152,
                    AgeRating = "12+",
                    FilePath = "/var/opt/mssql-files/films/harry_potter_1.mp4",
                    AudioTracks =
                    [
                        new AudioTrack { Language = "Русская", FilePath = "/var/opt/mssql-files/audio/harry_potter_1_ru.mp3" },
                        new AudioTrack { Language = "Оригинал (англ.)", FilePath = "/var/opt/mssql-files/audio/harry_potter_1_en.mp3" }
                    ],
                    Subtitles =
                    [
                        new Subtitle { Language = "Русские", FilePath = "/var/opt/mssql-files/subtitles/harry_potter_1_ru.vtt" },
                        new Subtitle { Language = "Английские", FilePath = "/var/opt/mssql-files/subtitles/harry_potter_1_en.vtt" }
                    ]
                },
                new Movie
                {
                    Title = "Крестный отец",
                    Description = "История итальянско-американской семьи мафиози и передачи власти от отца к сыну.",
                    Year = 1972,
                    DurationMinutes = 175,
                    AgeRating = "18+",
                    FilePath = "/var/opt/mssql-files/films/godfather.mp4",
                    AudioTracks =
                    [
                        new AudioTrack { Language = "Русская", FilePath = "/var/opt/mssql-files/audio/godfather_ru.mp3" },
                        new AudioTrack { Language = "Оригинал (англ.)", FilePath = "/var/opt/mssql-files/audio/godfather_en.mp3" }
                    ],
                    Subtitles =
                    [
                        new Subtitle { Language = "Русские", FilePath = "/var/opt/mssql-files/subtitles/godfather_ru.vtt" },
                        new Subtitle { Language = "Английские", FilePath = "/var/opt/mssql-files/subtitles/godfather_en.vtt" }
                    ]
                });

            await db.SaveChangesAsync();
        }
    }
}
