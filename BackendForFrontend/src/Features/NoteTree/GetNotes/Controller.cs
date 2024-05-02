using System.Net;
using Common;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.NoteTree.GetNotes;

[Route("api/[controller]")]
public partial class NoteTreeController(
    ISender sender
) : ApiControllerBase(sender)
{
    [HttpPost("GetNotes")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
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
