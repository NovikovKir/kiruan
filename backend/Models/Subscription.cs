using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Subscription
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [MaxLength(50)]
    public string Plan { get; set; } = "Месяц";

    public DateTime StartAt { get; set; } = DateTime.UtcNow;
    public DateTime EndAt { get; set; } = DateTime.UtcNow.AddMonths(1);

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Active";
}
