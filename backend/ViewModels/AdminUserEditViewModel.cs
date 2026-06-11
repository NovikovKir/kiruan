using System.ComponentModel.DataAnnotations;

namespace backend.ViewModels;

public class AdminUserEditViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "User";

    [Required]
    public string SubscriptionStatus { get; set; } = "Inactive";

    public DateTime? SubscriptionEndAt { get; set; }
}
