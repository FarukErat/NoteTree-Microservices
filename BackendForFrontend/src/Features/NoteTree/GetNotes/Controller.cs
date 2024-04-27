using Common;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.NoteTree.GetNotes;

[Route("api/NoteTree")]
public class GetNotesController(
    ISender sender)
    : ApiControllerBase(sender)
{
    [HttpPost("GetNotes")]
    public async Task<IActionResult> GetNotes()
    {
        string? sessionIdStr = HttpContext.Request.Cookies["SID"];
        if (sessionIdStr is null)
        {
            return Unauthorized("Session ID not found in cookies");
        }

        if (!Guid.TryParse(sessionIdStr, out Guid sessionId))
        {
            return Unauthorized("Session ID is not a valid GUID");
        }

        ErrorOr<GetNotesResponse> result = await Mediator.Send(new GetNotesRequest(sessionId));

        return result.Match(
            response => Ok(result.Value),
            ProblemDetails);
    }
}
