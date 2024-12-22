using System.ComponentModel.DataAnnotations;

public class AssistantViewModel
{

    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string selectedRole { get; set; }=null!;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
