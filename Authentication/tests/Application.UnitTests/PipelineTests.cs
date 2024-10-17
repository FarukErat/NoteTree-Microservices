using Application.Behaviors;
using ErrorOr;
using FluentAssertions;
using Mocks;
using Mocks.Mediator;
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
        MockRequest request = new(
            Username: "username",
            Password: "password",
            Email: "email", // invalid email
            FirstName: "first name",
            LastName: "last name"
        );

        // Act
        ErrorOr<MockResponse> response = await Validate(
            request,
            [new MockRequestValidator()]);

        // Assert
        response.IsError.Should().BeTrue("email is invalid");
    }

    [Fact]
    public async void ValidationBehavior_WhenRequestValid_ShouldValidate()
    {
        // Arrange
        MockRequest request = new(
            Username: "username",
            Password: "password",
            Email: "email@example.com",
            FirstName: "first name",
            LastName: "last name"
        );

        // Act
        ErrorOr<MockResponse> response = await Validate(
            request,
            [new MockRequestValidator()]);

        // Assert
        response.IsError.Should().BeFalse("request is valid");
    }

    [Fact]
    public async void LoggingBehavior_WhenRequestHandled_ShouldLog()
    {
        // Arrange
        MockRequest request = new(
            Username: "username",
            Password: "password",
            Email: "email@example.com",
            FirstName: "first name",
            LastName: "last name"
        );

        ILogger<LoggingBehavior<MockRequest, ErrorOr<MockResponse>>> logger
            = Substitute.For<ILogger<LoggingBehavior<MockRequest, ErrorOr<MockResponse>>>>();

        LoggingBehavior<MockRequest, ErrorOr<MockResponse>> loggingBehavior = new(logger);

        // Act
        await loggingBehavior.Handle(
            request,
            () => Task.FromResult(new ErrorOr<MockResponse>()),
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
        Assert.Contains($"Handling {nameof(MockRequest)}", firstLogCall[2]?.ToString());

        object?[] secondLogCall = logCalls.ElementAt(1).GetArguments();
        Assert.Contains($"Handled {nameof(MockRequest)}", secondLogCall[2]?.ToString());
    }
}
