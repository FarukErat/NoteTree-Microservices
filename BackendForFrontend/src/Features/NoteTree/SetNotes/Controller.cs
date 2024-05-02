using System.Net;
using Common;
using ErrorOr;
using Features.NoteTree.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.NoteTree.SetNotes;

[Route("api/[controller]")]
public partial class NoteTreeController(
    ISender sender
) : ApiControllerBase(sender)
{
    [HttpPost("SetNotes")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
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
            response => Ok(new { message = "Notes set successfully" }),
            ProblemDetails);
    }
}
