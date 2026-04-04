using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController
{
    [HttpGet]
    public string Get() => "Hello World!";
}