using Common;
using ErrorOr;
using Features.NoteTree.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.NoteTree.SetNotes;

[Route("api/NoteTree")]
public class SetNotesController(
    ISender sender)
    : ApiControllerBase(sender)
{
    [HttpPost("SetNotes")]
    public async Task<IActionResult> SetNotes(Note[] notes)
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

        ErrorOr<SetNotesResponse> result = await Mediator.Send(new SetNotesRequest(sessionId, notes));

        return result.Match(
            response => Ok("Notes set successfully"),
            ProblemDetails);
    }
}
