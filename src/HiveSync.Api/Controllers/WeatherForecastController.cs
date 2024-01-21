using HiveSync.Data.Entities;
using HiveSync.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace HiveSyncApi.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ITestClass _testService;
    private readonly IBlaService _blaService;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
            IBlaService blaService,
            ITestClass testService,
            ILogger<WeatherForecastController> logger
        )
    {
        _logger = logger;
        _blaService = blaService;
        _testService = testService;
    }

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
        return Ok();
    }
}
