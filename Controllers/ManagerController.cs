using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class ManagerController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly LibraryDbContext _context;
    public ManagerController(LibraryDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _context.books.Include(x => x.category).ToListAsync();
        return View(books);
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.books.FindAsync(id);
        if (book != null)
        {
            _context.books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View("Index");

    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewBag.categories = await _context.categories.ToListAsync();
        return View(await _context.books.FindAsync(id));
    }
    [HttpPost]
    public async Task<IActionResult> Search(string name)
    {

        var books = await _context.books
                           .Include(p => p.category)
                           .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                           .OrderBy(p => p.Name)
                           .ToListAsync();

        return View(books);
    }

    public async Task<IActionResult> toUser(int id)
    {
        var book = await _context.books.FindAsync(id);


        if (book == null)
        {
            TempData["message"] = "kitap İd si bulunamadı";
            return RedirectToAction("Index");
        }
        if (!book.IsAvailable)
        {
            TempData["message"] = "kitap zaten kullanımda";
            return RedirectToAction("Index");
        }
        var model = new toUserViewModel
        {
            BookId = book.BooksId
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> toUser(toUserViewModel model, int id)
    {
        if (model.Email != null)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var book = await _context.books.FindAsync(id);
            if (book == null)
            {
                TempData["Message"] = "Kitap bilgisi doğrulanamadı";
                return RedirectToAction("Index");
            }
            if (user == null)
            {
                TempData["Message"] = "Kullanıcı Bulunamadı";
                return RedirectToAction("touser");
            }

            book.AppUserId = user.Id;
            user.BorrowedBooks.Add(book);
            book.IsAvailable = false;

            await _context.SaveChangesAsync();

            TempData["Message"] = "Kitap başarıyla ödünç verildi";
            return RedirectToAction("Index");



        }

        TempData["Message"] = "kullanıcı bulunamadı";
        return RedirectToAction();
    }
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users
            .Include(u => u.BorrowedBooks)
            .ToListAsync();

        var userList = new List<AppUser>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("User"))
            {
                userList.Add(user);
            }
        }

        return View(userList);
    }
    public async Task<IActionResult> Assistants()
    {
        var Assistants = await _userManager.Users

            .ToListAsync();

        var AssistantList = new List<AppUser>();

        foreach (var user in Assistants)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Assistant"))
            {
                AssistantList.Add(user);
            }
        }

        return View(AssistantList);
    }
    public async Task<IActionResult> toLibrary(int id)
    {
        var book = await _context.books.FindAsync(id);
        if (book == null)
        {
            TempData["Message"] = "kullanıcı bulunamadı";
            return RedirectToAction("Index");
        }

        var user =await _userManager.Users.Include(u=>u.BorrowedBooks).FirstOrDefaultAsync(u=>u.Id==book.AppUserId);
        if (user==null)
        {
            TempData["message"]="kullanıcı bulunamadı";
            return RedirectToAction("Index");
        }
        user.BorrowedBooks.Remove(book);
        book.AppUserId = null;
        book.IsAvailable=true;
        await _context.SaveChangesAsync();
        TempData["Message"] = "Kitap başarıyla teslim alındı";
        return RedirectToAction("Index");
    }

}