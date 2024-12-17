using System.ComponentModel.DataAnnotations.Schema;

public class Category
{
     [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CategoryId   { get; set; }
    public string  Name { get; set; }=null!;
    public List<Books> books { get; set; }=new();

}