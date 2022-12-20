using Microsoft.AspNetCore.Mvc;

namespace HelloService.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloWorldController : ControllerBase {
    private readonly ILogger<HelloWorldController> _logger;

    public HelloWorldController(ILogger<HelloWorldController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "SayHello")]
    public string Get() {
        return "Hello, world!";
    }
}