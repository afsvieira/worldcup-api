using Microsoft.AspNetCore.Mvc;

namespace WorldCup.API.Controllers;

/// <summary>
/// Controller for public-facing web pages
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display the home page
    /// </summary>
    [HttpGet("/")]
    [HttpGet("/home")]
    [HttpGet("/home/index")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Display the API documentation page
    /// </summary>
    [HttpGet("/home/documentation")]
    [HttpGet("/docs")]
    public IActionResult Documentation()
    {
        return View();
    }

    /// <summary>
    /// Display the pricing plans page
    /// </summary>
    [HttpGet("/home/plans")]
    [HttpGet("/plans")]
    public IActionResult Plans()
    {
        return View();
    }
}
