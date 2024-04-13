using Common;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.Authentication.Register;

public class RegisterController(
    ISender sender)
    : ApiControllerBase(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create(RegisterRequestDto command)
    {
        ErrorOr<RegisterResponse> result = await Mediator.Send(new RegisterRequest(
            Username: command.Username,
            Password: command.Password,
            Email: command.Email,
            FirstName: command.FirstName,
            LastName: command.LastName
        ));
        return result.Match(
            response => Ok(new RegisterResponseDto(
                response.UserId)),
            ProblemDetails);
    }
}
