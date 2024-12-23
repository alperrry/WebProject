using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
// Controller for managing books, users, roles, and library-related functionalities.
public class ManagerController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly LibraryDbContext _context;

    public ManagerController(LibraryDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context; // Database context to interact with the library data.
        _userManager = userManager; // Manages user accounts.
        _roleManager = roleManager; // Manages user roles.
    }

    // Displays all books along with their associated categories.
    public async Task<IActionResult> Index()
    {
        // Includes related category data for each book.
        var books = await _context.books.Include(x => x.category).ToListAsync();
        return View(books);
    }

    // Deletes a book by its ID.
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.books.FindAsync(id); // Locates the book by ID.
        if (book != null)
        {
            _context.books.Remove(book); // Removes the book from the database.
            await _context.SaveChangesAsync(); // Saves changes to the database.
            return RedirectToAction("Index");
        }
        return View("Index");
    }

    // Displays the edit form for a specific book.
    public async Task<IActionResult> Edit(int id)
    {
        ViewBag.categories = await _context.categories.ToListAsync(); // Populates the dropdown with categories.
        return View(await _context.books.FindAsync(id)); // Finds the book to edit.
    }

    // Searches for books by name (case-insensitive).
    [HttpPost]
    public async Task<IActionResult> Search(string name)
    {
        // Filters books whose names contain the search term (ignores case).
        var books = await _context.books
            .Include(p => p.category) // Includes related category data.
            .Where(p => p.Name.ToLower().Contains(name.ToLower())) // Filters by name.
            .OrderBy(p => p.Name) // Orders books alphabetically.
            .ToListAsync();
        return View(books);
    }

    // Displays the form to assign a book to a user.
    public async Task<IActionResult> toUser(int id)
    {
        var book = await _context.books.FindAsync(id); // Finds the book by ID.
        if (book == null || !book.IsAvailable)
        {
            TempData["message"] = book == null ? "Book ID not found." : "Book is already in use.";
            return RedirectToAction("Index");
        }

        return View(new toUserViewModel { BookId = book.BooksId }); // Prepares the view model.
    }

    // Assigns a book to a user by their email.
    [HttpPost]
    public async Task<IActionResult> toUser(toUserViewModel model, int id)
    {
        if (model.Email != null)
        {
            var user = await _userManager.FindByEmailAsync(model.Email); // Finds user by email.
            var book = await _context.books.FindAsync(id); // Finds book by ID.

            if (book == null || user == null)
            {
                TempData["Message"] = book == null ? "Book information could not be verified." : "User not found.";
                return RedirectToAction("Index");
            }

            // Assigns the book to the user and marks it as unavailable.
            book.AppUserId = user.Id;
            user.BorrowedBooks.Add(book);
            book.IsAvailable = false;

            await _context.SaveChangesAsync(); // Saves changes to the database.
            TempData["Message"] = "Book successfully assigned.";
            return RedirectToAction("Index");
        }

        TempData["Message"] = "User not found.";
        return RedirectToAction("Index");
    }

    // Lists all users with the "User" role.
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.Include(u => u.BorrowedBooks).ToListAsync();
        var userList = new List<AppUser>();

        // Filters users who have the "User" role.
        foreach (var user in users)
        {
            if ((await _userManager.GetRolesAsync(user)).Contains("User"))
            {
                userList.Add(user);
            }
        }

        return View(userList);
    }

    // Lists all users with the "Assistant" role.
    public async Task<IActionResult> Assistants()
    {
        var assistants = await _userManager.Users.ToListAsync();
        var assistantList = new List<AppUser>();

        // Filters users who have the "Assistant" role.
        foreach (var user in assistants)
        {
            if ((await _userManager.GetRolesAsync(user)).Contains("Assistant"))
            {
                assistantList.Add(user);
            }
        }

        return View(assistantList);
    }

    // Returns a borrowed book to the library.
    public async Task<IActionResult> toLibrary(int id)
    {
        var book = await _context.books.FindAsync(id); // Finds the book by ID.
        if (book == null)
        {
            TempData["Message"] = "Book not found.";
            return RedirectToAction("Index");
        }

        // Finds the user who borrowed the book and removes the association.
        var user = await _userManager.Users.Include(u => u.BorrowedBooks).FirstOrDefaultAsync(u => u.Id == book.AppUserId);
        if (user != null)
        {
            user.BorrowedBooks.Remove(book);
        }

        book.AppUserId = null; // Resets the user association.
        book.IsAvailable = true; // Marks the book as available.
        await _context.SaveChangesAsync();
        TempData["Message"] = "Book successfully returned.";
        return RedirectToAction("Index");
    }

    // Displays the form to create a new user and assign them a role.
    public IActionResult Createuser()
    {
        ViewBag.Roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList(); // Prepares the role list.
        return View();
    }

    // Creates a new user and assigns them a role.
    [HttpPost]
    public async Task<IActionResult> Createuser(AssistantViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password); // Creates the user.

            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(model.selectedRole); // Finds the selected role.
                if (role != null)
                {
                    await _userManager.AddToRoleAsync(user, role.Name); // Assigns the role to the user.
                    TempData["message"] = "User successfully created.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Selected role not found.");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }

        ViewBag.Roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList();
        return View(model);
    }

    // Displays the form to create a new book.
    public IActionResult Createbook()
    {
        ViewBag.categories = _context.categories.Select(r => new { r.Name, r.CategoryId }).ToList(); // Prepares the category list.
        return View();
    }

    // Creates a new book and associates it with a category.
    [HttpPost]
    public async Task<IActionResult> Createbook(BookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.categories = _context.categories.Select(r => new { r.Name, r.CategoryId }).ToList();
            return View(model);
        }

        var category = await _context.categories.FirstOrDefaultAsync(c => c.CategoryId == model.CategoryId); // Finds the category.
        if (category == null)
        {
            ModelState.AddModelError("", "The specified category does not exist.");
            ViewBag.categories = _context.categories.Select(r => new { r.Name, r.CategoryId }).ToList();
            return View(model);
        }

        var book = new Books { Name = model.name, CategoryId = category.CategoryId }; // Creates a new book.
        await _context.books.AddAsync(book); // Adds the book to the database.
        await _context.SaveChangesAsync(); // Saves changes to the database.

        TempData["SuccessMessage"] = "The book has been successfully created.";
        return RedirectToAction("Index");
    }
}
