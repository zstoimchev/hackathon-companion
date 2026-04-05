using HackathonOS.Domain;
using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.API;

public class HackathonController : ControllerBase
{
    protected ActionResult<T> MapToActionResult<T>(GdgResult<T> result)
    {
        if (result.IsSuccessful) return Ok(result.Result);
        if (string.IsNullOrEmpty(result.Message)) return StatusCode((int)result.StatusCode);
        return StatusCode((int)result.StatusCode, new ErrorResult { Message = result.Message });
    }
}