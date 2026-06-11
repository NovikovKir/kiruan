using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Movie
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(1, 500)]
    public int DurationMinutes { get; set; }

    [Required]
    [MaxLength(20)]
    [RegularExpression("^[+][0-9]{1,2}$", ErrorMessage = "Age rating can only contain a plus sign and a number")]
    public string AgeRating { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    public List<AudioTrack> AudioTracks { get; set; } = new();
    public List<Subtitle> Subtitles { get; set; } = new();
}
