using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class LibraryDbContext:IdentityDbContext<AppUser>
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options):base(options)
    {
        
    }
   public   DbSet<Books> books =>Set<Books>();
   public   DbSet<Category> categories =>Set<Category>();
   
}