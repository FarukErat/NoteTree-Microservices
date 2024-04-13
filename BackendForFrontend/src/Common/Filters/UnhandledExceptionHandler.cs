using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.Filters;

public class ExceptionHandlerFilterAttribute(
    ILogger<ExceptionHandlerFilterAttribute> logger) : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionHandlerFilterAttribute> _logger = logger;
    public override void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception,
            "An unexpected error occurred while processing the request: {Path}",
            context.HttpContext.Request.Path);

        ProblemDetails problemDetails = new()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "An unexpected error occurred while processing your request.",
            Status = (int)HttpStatusCode.InternalServerError,
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new ObjectResult(problemDetails);

        context.ExceptionHandled = true;
    }
}
