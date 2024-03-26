using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.Controllers;

[ApiController]
public class ProblemDetails : ControllerBase
{
    protected ActionResult Problem(List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    private ObjectResult Problem(Error error)
    {
        int statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Problem(statusCode: statusCode,
            title: error.Description,
            detail: error.Code,
            instance: Request.Path);
    }

    private ActionResult ValidationProblem(List<Error> errors)
    {
        ModelStateDictionary modelStateDictionary = new();

        errors.ForEach(error => modelStateDictionary.AddModelError(error.Code, error.Description));

        return ValidationProblem(modelStateDictionary);
    }
}
