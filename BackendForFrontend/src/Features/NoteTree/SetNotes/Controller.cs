using Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.NoteTree.SetNotes;

[Route("api/NoteTree")]
public class SetNotesController(
    ISender sender)
    : ApiControllerBase(sender)
{
    [HttpPost("SetNotes")]
    public async Task<IActionResult> Create()
    {
        await Task.CompletedTask;
        return Ok("SetNotes");
    }
}
