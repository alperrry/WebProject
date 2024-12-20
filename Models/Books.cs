public class Books
{
    public int BooksId { get; set; }
    public string Name { get; set; }=null!;
    public int CategoryId { get; set; }
    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public bool IsAvailable { get; set; }=true;
    public Category category { get; set; }=null!;
}