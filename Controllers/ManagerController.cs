using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class ManagerController:Controller
{
    private readonly LibraryDbContext _context;
    public ManagerController(LibraryDbContext context)
    {
        _context=context;
    }

    public async Task<IActionResult> Index()
    {   var books =await _context.books.Include(x=>x.category).ToListAsync();
         return View(books);
    }

    
    public async Task<IActionResult> Details(int id)
    {    
        
        return View(await _context.books.Include(x => x.category).FirstOrDefaultAsync(x => x.BooksId == id));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var book=await _context.books.FindAsync(id);
        if (book!=null)
        {
            _context.books.Remove(book);
           await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View("Index");
        
    }

    public async Task<IActionResult> Edit(int id)
    {   
        ViewBag.categories=await _context.categories.ToListAsync();
        return View(await _context.books.FindAsync(id));
    }
}