using Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.NoteTree.GetNotes;

[Route("api/NoteTree")]
public class GetNotesController(
    ISender sender)
    : ApiControllerBase(sender)
{
    [HttpPost("GetNotes")]
    public async Task<IActionResult> Create()
    {
        await Task.CompletedTask;
        return Ok("GetNotes");
    }
}
