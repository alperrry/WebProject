using System.ComponentModel.DataAnnotations;

public class BookViewModel 
{
    [Required]
    public string name { get; set; }= null!;
    [Required]
    public int CategoryId { get; set; }
    
}