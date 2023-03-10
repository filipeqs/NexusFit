using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NexusFit.Auth.API.Dtos;
using NexusFit.Auth.API.Helpers;
using NexusFit.Auth.FunctionalTests.Extensions;
using NexusFit.Auth.FunctionalTests.Helpers;
using NexusFit.Auth.FunctionalTests.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NexusFit.Auth.FunctionalTests.Endpoints;

[Collection("Test Collection")]
public class AuthControllerOauthLoginTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;
    private readonly IdentityServerSettings _identitySettings;

    public AuthControllerOauthLoginTests(AuthApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabaseAsync;
        var identityOptions = apiFactory.Services.GetService<IOptions<IdentityServerSettings>>();
        _identitySettings = identityOptions.Value;
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

    private FormUrlEncodedContent CreateLoginRequest(string userName, string password)
    {
        var requestContent = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", userName),
            new KeyValuePair<string, string>("password", password),
            new KeyValuePair<string, string>("scope", $"{_identitySettings.Scopes}")
        };
        return new FormUrlEncodedContent(requestContent);
    }

    private string CreateAuthenticationString()
    {
        var authenticationString = $"{_identitySettings.ClientId}:{_identitySettings.Secret}";
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
    }

    [Fact]
    public async Task LoginOAuth_ShouldReturn_Token()
    {
        var requestContentEncoded = CreateLoginRequest("test@email.com", "P@ssw0rd");

        var base64EncodedAuthenticationString = CreateAuthenticationString();

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, AuthRoutes.Post.OAuthLogin);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        requestMessage.Content = requestContentEncoded;

        var response = await _client.SendAsync(requestMessage);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var token = await response.SerializeResponse<Token>();

        token.Should().NotBeNull();
        token.AccessToken.Should().NotBeNullOrWhiteSpace();
        token.TokenType.Should().NotBeNullOrWhiteSpace();
        token.ExpiresIn.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task LoginOAuthUnexisting_ShouldReturn_EmptyToken()
    {
        var requestContentEncoded = CreateLoginRequest("unexting@email.com", "P@ssw0rd");

        var base64EncodedAuthenticationString = CreateAuthenticationString();

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, AuthRoutes.Post.OAuthLogin);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        requestMessage.Content = requestContentEncoded;

        var response = await _client.SendAsync(requestMessage);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var token = await response.SerializeResponse<Token>();

        token.Should().NotBeNull();
        token.AccessToken.Should().BeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LoginOAuthWrongPassword_ShouldReturn_EmptyToken()
    {
        var requestContentEncoded = CreateLoginRequest("test@email.com", "P@ssw0rdWrong");

        var base64EncodedAuthenticationString = CreateAuthenticationString();

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, AuthRoutes.Post.OAuthLogin);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        requestMessage.Content = requestContentEncoded;

        var response = await _client.SendAsync(requestMessage);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var token = await response.SerializeResponse<Token>();

        token.Should().NotBeNull();
        token.AccessToken.Should().BeNullOrWhiteSpace();
    }
}
