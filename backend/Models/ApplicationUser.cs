using Microsoft.AspNetCore.Identity;

namespace backend.Models;

public class ApplicationUser : IdentityUser
{
    public string Role { get; set; } = "User";
    public string SubscriptionStatus { get; set; } = "Inactive";
    public DateTime? SubscriptionEndAt { get; set; }
}
