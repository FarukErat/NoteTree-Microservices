using System.Security.Claims;
using Application.Interfaces.Infrastructure;
using Application.Interfaces.Persistence;
using Domain.Models;
using ErrorOr;
using MediatR;

namespace Application.UseCases.GetNotes;

public sealed class GetNotesHandler(
    INoteReadRepository noteReadRepository,
    IJwtHelper jwtHelper
) : IRequestHandler<GetNotesRequest, ErrorOr<GetNotesResponse>>
{
    private readonly INoteReadRepository _noteReadRepository = noteReadRepository;
    private readonly IJwtHelper _jwtHelper = jwtHelper;

    public async Task<ErrorOr<GetNotesResponse>> Handle(GetNotesRequest request, CancellationToken cancellationToken)
    {
        Guid? UserId = _jwtHelper.ExtractUserId(request.Jwt);
        if (UserId is null)
        {
            return Error.Validation(description: "Invalid JWT");
        }

        Note[]? notes = await _noteReadRepository.GetByIdAsync((Guid)UserId);
        if (notes is null)
        {
            return Error.NotFound(description: "Note record not found");
        }

        return new GetNotesResponse(notes);
    }
}
