using FluentAssertions;
using NexusFit.Auth.API.Dtos;
using NexusFit.Auth.FunctionalTests.Extensions;
using NexusFit.Auth.FunctionalTests.Helpers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NexusFit.Auth.FunctionalTests.Endpoints;

[Collection("Test Collection")]
public class AuthControllerUserExistsTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public AuthControllerUserExistsTests(AuthApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabaseAsync;
    }

    public async Task InitializeAsync()
    {
        var registerDto = new RegisterDto
        {
            Email = "test@email.com",
            Password = "P@ssw0rd",
            FirstName = "Test",
            LastName = "Test",
        };

        var registerRequest = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
        await _client.PostAsync(AuthRoutes.Post.Register, registerRequest);
    }

    public async Task DisposeAsync() => await _resetDatabase();

    [Fact]
    public async Task ExistingUser_ShouldReturn_True()
    {
        var email = "test@email.com";

        var response = await _client.GetAsync(AuthRoutes.Get.UserExists(email));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userExists = await response.SerializeResponse<bool>();

        userExists.Should().BeTrue();
    }

    [Fact]
    public async Task UnexistingUser_ShouldReturn_False()
    {
        var email = "random@email.com";

        var response = await _client.GetAsync(AuthRoutes.Get.UserExists(email));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userExists = await response.SerializeResponse<bool>();

        userExists.Should().BeFalse();
    }
}
