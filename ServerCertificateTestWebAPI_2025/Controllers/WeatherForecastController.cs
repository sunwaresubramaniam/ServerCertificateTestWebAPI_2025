using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTLS_Cert_Lib;
using ServerCertificateTestWebAPI_2025.Services;


namespace ServerCertificateTestWebAPI_2025.Controllers;
//[Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

   // private readonly ILogger<WeatherForecastController> _logger;

    private readonly HttpContextService _contextService;

    private readonly TFLoggerService _logger;

    public WeatherForecastController(HttpContextService contextService, TFLoggerService logger)
    {
        _contextService = contextService;
        _logger = logger;
    }


    //public WeatherForecastController(HttpContextService contextService,ILogger<WeatherForecastController> logger)
    //{
    //    _contextService = contextService;
    //    _logger = logger;
    //}

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
       _logger.LogInformation("Calling the Get method.");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("secure-data")]
    public IActionResult GetSecureData()
    {
        _logger.LogInformation("Calling the Get Secure Data.");
        var clientCert = HttpContext.Connection.ClientCertificate;
        if (clientCert == null)
        {
            _logger.LogError("Error:Client certificate is required.");
            return Forbid("Client certificate is required.");
        }
        return Ok($"Secure data for certificate: {clientCert.Subject}");
    }
}
