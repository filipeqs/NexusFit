using FluentAssertions;
using NexusFit.Auth.API.Dtos;
using NexusFit.Auth.FunctionalTests.Helpers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NexusFit.Auth.FunctionalTests.Endpoints;

[Collection("Test Collection")]
public class AuthControllerRegister : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public AuthControllerRegister(AuthApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _resetDatabase();

    [Fact]
    public async Task RegisterUser_ShouldReturn_Token()
    {
        var registerDto = new RegisterDto
        {
            Email = "test@email.com",
            Password = "P@ssw0rd",
            FirstName = "Test",
            LastName = "Test",
        };

        var request = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(AuthRoutes.Post.Register, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var user = JsonSerializer.Deserialize<UserDto>(content, options);

        user.Should().NotBeNull();
        user.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RegisterExistingUser_ShouldReturn_BadRequest()
    {
        var registerDto = new RegisterDto
        {
            Email = "test@email.com",
            Password = "P@ssw0rd",
            FirstName = "Test",
            LastName = "Test",
        };

        var request = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(AuthRoutes.Post.Register, request);
        var duplicatedResponse = await _client.PostAsync(AuthRoutes.Post.Register, request);

        duplicatedResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterInvalidEmail_ShouldReturn_BadRequest()
    {
        var registerDto = new RegisterDto
        {
            Email = "test@",
            Password = "P@ssw0rd",
            FirstName = "Test",
            LastName = "Test",
        };

        var request = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(AuthRoutes.Post.Register, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterInvalidPassword_ShouldReturn_BadRequest()
    {
        var registerDto = new RegisterDto
        {
            Email = "test@email.com",
            Password = "password",
            FirstName = "Test",
            LastName = "Test",
        };

        var request = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(AuthRoutes.Post.Register, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
