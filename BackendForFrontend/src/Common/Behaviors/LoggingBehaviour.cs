using System.Diagnostics;
using MediatR;

namespace Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    private readonly ILogger _logger = logger;
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {@Request}", request);
        _timer.Start();
        TResponse result = await next();
        _timer.Stop();
        _logger.LogInformation("Handled {@Request} in {ElapsedMilliseconds}ms", request, _timer.ElapsedMilliseconds);

        return result;
    }
}
