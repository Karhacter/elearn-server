using elearn_server.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromResult<T>(ServiceResult<T> result)
    {
        var response = new ApiResponse<T>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data,
            Errors = result.Errors,
            Meta = result.Meta
        };

        return StatusCode(result.StatusCode, response);
    }
}
