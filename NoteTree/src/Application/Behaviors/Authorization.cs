using System.Reflection;
using System.Text.Json;
using Application.Interfaces.Infrastructure;
using Application.Security;
using Domain.Enums;
using ErrorOr;
using MediatR;
using Security;

namespace Application.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>(
    IJwtHelper jwtHelper
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizeableRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IJwtHelper _jwtHelper = jwtHelper;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ErrorOr<Success> result = await _jwtHelper.VerifyTokenAsync(request.Jwt);
        if (result.IsError)
        {
            return (dynamic)result.Errors;
        }

        List<AuthorizeAttribute> authorizationAttributes = request
            .GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();
        if (authorizationAttributes.Count == 0)
        {
            return await next();
        }

        List<Role> requiredRoles = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Roles ?? [])
            .ToList();
        List<Role> userRoles = _jwtHelper.GetUserRoles(request.Jwt) ?? [];

        if (!IsInRole(userRoles, requiredRoles))
        {
            return (dynamic)Error.Unauthorized(description: "User does not have required roles");
        }

        return await next();
    }

    private static bool IsInRole(List<Role> userRoles, List<Role> requiredRoles)
    {
        return requiredRoles.Any(userRoles.Contains);
    }
}
