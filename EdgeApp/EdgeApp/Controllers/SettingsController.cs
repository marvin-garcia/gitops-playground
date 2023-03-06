using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EdgeApp.Models;
using Microsoft.Extensions.Configuration;

namespace EdgeApp.Controllers;

public class SettingsController : Controller
{
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(ILogger<SettingsController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Set settings
        Dictionary<string, string> settings = new Dictionary<string, string>()
        {
            { "temperature", Environment.GetEnvironmentVariable("TEMPERATURE") },
            { "pressure", Environment.GetEnvironmentVariable("PRESSURE") },
            { "velocity", Environment.GetEnvironmentVariable("VELOCITY") },
        };
        ViewData["Settings"] = settings;
        
        // Set secrets
        Dictionary<string, string> secrets = new Dictionary<string, string>()
        {
            { "token", Environment.GetEnvironmentVariable("TOKEN") },
            { "username", Environment.GetEnvironmentVariable("USERNAME") },
            { "password", Environment.GetEnvironmentVariable("PASSWORD") },
        };
        ViewData["Secrets"] = secrets;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
