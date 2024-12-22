using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private SignInManager<AppUser> _signInManager;


    public AccountController (UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }



    public IActionResult Login()

    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                await _signInManager.SignOutAsync();
              
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    await _userManager.SetLockoutEndDateAsync(user, null);

                    return RedirectToAction("Index", "Manager");
                }
                else if (result.IsLockedOut)
                {
                    var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeleft = lockoutDate.Value - DateTime.UtcNow;
                    ModelState.AddModelError("", $"Hesabınız kilitlendi ,Lütfen {timeleft.Minutes} dakika sonra deneyiniz. ");
                }
                else
                {
                    ModelState.AddModelError("", "hatalı  parola");
                }
            }
            else
            {
                ModelState.AddModelError("", "hatalı email");
            }
        }
        return View(model);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = new AppUser { UserName = model.UserName, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {            await _userManager.AddToRoleAsync(user, "User");

            TempData["message"] = "Kullanıcı başarıyla oluşturuldu.";
            return RedirectToAction("Index", "Manager");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description); // Hataları ModelState'e ekle
            }
        }
    }

    return View(model); // Hatalar varsa aynı sayfaya dön
}

public async Task<IActionResult> Logout()
{
    // Oturumdan çıkış yap
    await _signInManager.SignOutAsync();

    // Kullanıcıyı giriş sayfasına yönlendir
    return RedirectToAction("Login", "Account");
}

  
}