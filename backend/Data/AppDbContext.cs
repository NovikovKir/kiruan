using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<AudioTrack> AudioTracks => Set<AudioTrack>();
    public DbSet<Subtitle> Subtitles => Set<Subtitle>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<Movie>().ToTable("Movies");
        builder.Entity<AudioTrack>().ToTable("Audio_tracks");
        builder.Entity<Subtitle>().ToTable("Subtitles");
        builder.Entity<Subscription>().ToTable("Subscritptions");

        builder.Entity<Movie>()
            .HasMany(movie => movie.AudioTracks)
            .WithOne(track => track.Movie)
            .HasForeignKey(track => track.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Movie>()
            .HasMany(movie => movie.Subtitles)
            .WithOne(subtitle => subtitle.Movie)
            .HasForeignKey(subtitle => subtitle.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .Property(user => user.Role)
            .HasMaxLength(20);

        builder.Entity<ApplicationUser>()
            .Property(user => user.SubscriptionStatus)
            .HasMaxLength(50);
    }
}
