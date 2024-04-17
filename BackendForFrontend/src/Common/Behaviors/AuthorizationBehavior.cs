using System.Reflection;
using Common.Interfaces;
using Common.Security;
using Domain.Enums;
using ErrorOr;
using MediatR;

namespace Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(
    ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly ICurrentUserService _currentUser = currentUserService;

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        AuthorizeAttribute[] authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToArray();

        if (authorizeAttributes.Length == 0)
        {
            return next();
        }

        Role[] requiredRoles = authorizeAttributes
            .SelectMany(x => x.Roles ?? []).ToArray();

        Permission[] requiredPermissions = authorizeAttributes
            .SelectMany(x => x.Permissions ?? []).ToArray();

        Policy[] requiredPolicies = authorizeAttributes
            .SelectMany(x => x.Policies ?? []).ToArray();

        bool isAuthorized = _currentUser.AuthorizeUser(
            requiredRoles,
            requiredPermissions,
            requiredPolicies);

        if (!isAuthorized)
        {
            return (dynamic)Error.Forbidden(description: "The user is not authorized to perform this action.");
        }

        return next();
    }
}
