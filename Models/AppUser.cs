using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public ICollection<Books> BorrowedBooks { get; set; } = new List<Books>();
}