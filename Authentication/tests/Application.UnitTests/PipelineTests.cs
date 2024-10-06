using Application.UseCases.Register;
using ErrorOr;
using FluentAssertions;
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
}
