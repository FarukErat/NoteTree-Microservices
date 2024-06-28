using Application.UseCases.Register;
using ErrorOr;
using FluentAssertions;
using static Mocks.MockServices;

namespace Tests;

public class AuthenticationTests
{
    [Fact]
    public async void RegisterHandler_WhenInvalidCredentials_ShouldReturnError()
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
        ErrorOr<RegisterResponse> response = await Sender.Send(request);

        // Assert
        response.IsError.Should().BeTrue("email is invalid");
    }
}
