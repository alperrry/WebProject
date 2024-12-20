using System.ComponentModel.DataAnnotations;

public class toUserViewModel
{
    [EmailAddress]
    public string? Email { get; set; }
    public int BookId { get; set; }
}
