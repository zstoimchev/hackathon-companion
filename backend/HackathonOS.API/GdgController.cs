using HackathonOS.Domain;
using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.API;

public class GdgController : ControllerBase
{
    protected ActionResult<T> MapToActionResult<T>(GdgResult<T> result)
    {
        if (result.IsSuccessful) return Ok(result.Result);
        if (string.IsNullOrEmpty(result.Message)) return StatusCode((int)result.StatusCode);
        return StatusCode((int)result.StatusCode, new ErrorResult { Message = result.Message });
    }

    protected Guid ParseGuid
    {
        get
        {
            const string routeKey = "id";

            if (HttpContext.Request.RouteValues.TryGetValue(routeKey, out var value))
            {
                var idString = value?.ToString();
                if (!string.IsNullOrEmpty(idString) && Guid.TryParse(idString, out var guid))
                {
                    return guid;
                }
            }

            throw new ArgumentException("Invalid or missing GUID in the request path.");
        }
    }
}