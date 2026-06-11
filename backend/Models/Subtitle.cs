using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Subtitle
{
    public int Id { get; set; }

    [Required]
    public int MovieId { get; set; }

    public Movie? Movie { get; set; }

    [Required]
    [MaxLength(100)]
    public string Language { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
}
