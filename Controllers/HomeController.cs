using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProject.Models;

namespace WebProject.Controllers;

// This is the HomeController, which manages the main views of the application.
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    // Constructor to inject the logger dependency for logging purposes.
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Action to render the Index view (usually the homepage).
    public IActionResult Index()
    {
        return View();
    }

    // GET: Contactus - Displays the contact form to the user.
    [HttpGet]
    public IActionResult Contactus()
    {
        return View();
    }

    // POST: Contactus - Processes the submitted contact form data.
    [HttpPost]
    public IActionResult Contactus(string Name, string Email, string Message)
    {
        // Process the form data (e.g., save to database or send an email).
        // For now, just return a success message to the view.
        ViewData["Message"] = "Your message has been sent successfully!";

        // Reload the same page with the success message.
        return View();
    }
}
