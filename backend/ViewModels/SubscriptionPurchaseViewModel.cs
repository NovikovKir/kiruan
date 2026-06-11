using System.ComponentModel.DataAnnotations;

namespace backend.ViewModels;

public class SubscriptionPurchaseViewModel
{
    [Required]
    [MaxLength(50)]
    public string Plan { get; set; } = "Месяц";

    [Required]
    [CreditCard]
    public string CardNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string CardHolder { get; set; } = string.Empty;

    [Required]
    [Range(1, 12)]
    public int ExpMonth { get; set; }

    [Required]
    [Range(2024, 2100)]
    public int ExpYear { get; set; }

    [Required]
    [Range(100, 999)]
    public int Cvv { get; set; }
}
