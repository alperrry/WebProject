using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private SignInManager<AppUser> _signInManager;

    // Constructor to initialize dependencies for user and role management
    public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    // Display the login page
    public IActionResult Login()
    {
        return View();
    }

    // Handle the login process
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid) // Check if the provided model is valid
        {
            var user = await _userManager.FindByEmailAsync(model.Email); // Find user by email

            if (user != null)
            {
                await _signInManager.SignOutAsync(); // Sign out any existing user

                // Attempt to sign in the user
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user); // Reset failed access attempts
                    await _userManager.SetLockoutEndDateAsync(user, null); // Clear lockout end date

                    return RedirectToAction("Index", "Manager"); // Redirect to the manager's index page
                }
                else if (result.IsLockedOut)
                {
                    // If the account is locked, calculate the remaining lockout time
                    var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeleft = lockoutDate.Value - DateTime.UtcNow;
                    ModelState.AddModelError("", $"Your account is locked. Please try again in {timeleft.Minutes} minutes.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid password.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid email.");
            }
        }
        return View(model); // Return the view with validation errors if any
    }

    // Display the user creation page
    public IActionResult Create()
    {
        return View();
    }

    // Handle the creation of a new user
    [HttpPost]
    public async Task<IActionResult> Create(CreateViewModel model)
    {
        if (ModelState.IsValid) // Check if the provided model is valid
        {
            var user = new AppUser { UserName = model.UserName, Email = model.Email }; // Create a new user object
            var result = await _userManager.CreateAsync(user, model.Password); // Save the user in the database

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User"); // Assign the "User" role to the new user

                TempData["message"] = "User successfully created."; // Display a success message
                return RedirectToAction("Index", "Manager"); // Redirect to the manager's index page
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description); // Add any errors to the model state
                }
            }
        }

        return View(model); // Return the view with validation errors if any
    }

    // Log out the current user
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync(); // Sign out the current user

        return RedirectToAction("Login", "Account"); // Redirect to the login page
    }
}
