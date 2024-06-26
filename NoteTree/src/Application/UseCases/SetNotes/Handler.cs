using System.Security.Claims;
using Application.Interfaces.Infrastructure;
using Application.Interfaces.Persistence;
using Domain.Models;
using ErrorOr;
using MediatR;

namespace Application.UseCases.SetNotes;

public sealed class SetNotesHandler(
    INoteReadRepository noteReadRepository,
    INoteWriteRepository noteWriteRepository,
    IJwtHelper jwtHelper
) : IRequestHandler<SetNotesRequest, ErrorOr<SetNotesResponse>>
{
    private readonly INoteReadRepository _noteReadRepository = noteReadRepository;
    private readonly INoteWriteRepository _noteWriteRepository = noteWriteRepository;
    private readonly IJwtHelper _jwtHelper = jwtHelper;

    public async Task<ErrorOr<SetNotesResponse>> Handle(SetNotesRequest request, CancellationToken cancellationToken)
    {
        Guid? UserId = _jwtHelper.ExtractUserId(request.Jwt);
        if (UserId is null)
        {
            return Error.Validation(description: "Invalid JWT");
        }

        Note[]? existingNotes = await _noteReadRepository.GetByIdAsync((Guid)UserId);
        if (existingNotes is null)
        {
            return Error.NotFound(description: "Note record not found");
        }

        await _noteWriteRepository.UpdateAsync((Guid)UserId, request.Notes);

        return new SetNotesResponse();
    }
}
