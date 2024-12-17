public class Books
{
    public int BooksId { get; set; }
    public string Name { get; set; }=null!;
    public int CategoryId { get; set; }
    public bool IsAvailable { get; set; }=false;
    public Category category { get; set; }=null!;
}