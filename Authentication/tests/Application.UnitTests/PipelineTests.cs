using Application.Behaviors;
using Application.UseCases.Register;
using ErrorOr;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.Core;
using static Mocks.MockServices;

namespace Tests;

public class PipelineTests
{
    [Fact]
    public async void ValidationBehavior_WhenRequestMalformed_ShouldInvalidate()
    {
        // Arrange
        RegisterRequest request = new(
            Username: "username",
            Password: "password",
            Email: "email", // invalid email
            FirstName: "first name",
            LastName: "last name"
        );

        // Act
        ErrorOr<RegisterResponse> response = await Validate(
            request,
            [new RegisterRequestValidator()]);

        // Assert
        response.IsError.Should().BeTrue("email is invalid");
    }

    [Fact]
    public async void ValidationBehavior_WhenRequestValid_ShouldValidate()
    {
        // Arrange
        RegisterRequest request = new(
            Username: "username",
            Password: "password",
            Email: "email@example.com",
            FirstName: "first name",
            LastName: "last name"
        );

        // Act
        ErrorOr<RegisterResponse> response = await Validate(
            request,
            [new RegisterRequestValidator()]);

        // Assert
        response.IsError.Should().BeFalse("request is valid");
    }

    [Fact]
    public async void LoggingBehavior_WhenRequestHandled_ShouldLog()
    {
        // Arrange
        RegisterRequest request = new(
            Username: "username",
            Password: "password",
            Email: "email@example.com",
            FirstName: "first name",
            LastName: "last name"
        );

        ILogger<LoggingBehavior<RegisterRequest, ErrorOr<RegisterResponse>>> logger
            = Substitute.For<ILogger<LoggingBehavior<RegisterRequest, ErrorOr<RegisterResponse>>>>();

        LoggingBehavior<RegisterRequest, ErrorOr<RegisterResponse>> loggingBehavior = new(logger);

        // Act
        await loggingBehavior.Handle(
            request,
            () => Task.FromResult(new ErrorOr<RegisterResponse>()),
            CancellationToken.None);

        // Assert
        logger.ReceivedWithAnyArgs(2).Log(
                Arg.Any<LogLevel>(),
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>());

        IEnumerable<ICall> logCalls = logger.ReceivedCalls();

        object?[] firstLogCall = logCalls.ElementAt(0).GetArguments();
        Assert.Contains("Handling RegisterRequest", firstLogCall[2]?.ToString());

        object?[] secondLogCall = logCalls.ElementAt(1).GetArguments();
        Assert.Contains("Handled RegisterRequest", secondLogCall[2]?.ToString());
    }
}
