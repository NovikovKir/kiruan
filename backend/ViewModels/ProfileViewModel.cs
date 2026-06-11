using System.ComponentModel.DataAnnotations;

namespace backend.ViewModels;

public class ProfileViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [MinLength(6)]
    public string? NewPassword { get; set; }
}
