using Application.Behaviors;
using Application.UseCases.Register;
using ErrorOr;
using FluentAssertions;
using MediatR;
using NSubstitute;
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
        logger.Received().LogInformation("Handling {@Request}", request);
        logger.Received().LogInformation("Handled {@Request} in {ElapsedMilliseconds}ms", request, Arg.Any<long>());
    }
}
