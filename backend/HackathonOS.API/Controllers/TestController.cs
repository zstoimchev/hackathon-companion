using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public string Get() => "Hello Wor------------------------------------------------------------ld!";
}