using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EdgeApp.Models;
using Microsoft.Extensions.Configuration;

namespace EdgeApp.Controllers;

public class SettingsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(IConfiguration configuration, ILogger<SettingsController> logger)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        // Set settings
        Dictionary<string, string> settings = new Dictionary<string, string>()
        {
            { "temperature", _configuration["TEMPERATURE"] },
            { "pressure", _configuration["PRESSURE"] },
            { "velocity", _configuration["VELOCITY"] },
        };
        ViewData["Settings"] = settings;
        
        // Set secrets
        Dictionary<string, string> secrets = new Dictionary<string, string>()
        {
            { "token", _configuration["TOKEN"] },
            { "username", _configuration["USERNAME"] },
            { "password", _configuration["PASSWORD"] },
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
