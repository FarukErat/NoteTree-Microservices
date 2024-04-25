using System.Reflection;
using Application.Interfaces.Infrastructure;
using Application.Security;
using ErrorOr;
using MediatR;
using Security;

namespace Application.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(
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
        ErrorOr<Success> result = _jwtHelper.VerifyToken(request.Jwt);
        if (result.IsError)
        {
            return (dynamic)result.Errors;
        }
        return await next();

        // AuthorizeAttribute[] authorizationAttributes = request
        //     .GetType()
        //     .GetCustomAttributes<AuthorizeAttribute>()
        //     .ToArray();

        // if (authorizationAttributes.Length == 0)
        // {
        //     return await next();
        // }

        // string[] requiredPermissions = authorizationAttributes
        //     .SelectMany(authorizationAttribute => authorizationAttribute.Permissions?.Split(',') ?? [])
        //     .ToArray();

        // string[] requiredRoles = authorizationAttributes
        //     .SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? [])
        //     .ToArray();

        // string[] requiredPolicies = authorizationAttributes
        //     .SelectMany(authorizationAttribute => authorizationAttribute.Policies?.Split(',') ?? [])
        //     .ToArray();

        // return await next();
    }
}
