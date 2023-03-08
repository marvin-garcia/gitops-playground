using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EdgeApp.Models;

namespace EdgeApp.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        // Build version
        ViewData["AppVersion"] = "1.0.0";
        
        // Settings
        Dictionary<string, string> settings = new Dictionary<string, string>()
        {
            { "temperature", _configuration["TEMPERATURE"] },
            { "pressure", _configuration["PRESSURE"] },
            { "velocity", _configuration["VELOCITY"] },
        };
        ViewData["Settings"] = settings;
        
        // Secrets
        Dictionary<string, string> secrets = new Dictionary<string, string>()
        {
            { "token", _configuration["TOKEN"] },
            { "username", _configuration["USERNAME"] },
            { "password", _configuration["PASSWORD"] },
        };
        ViewData["Secrets"] = secrets;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
