﻿using FluentAssertions;
using NexusFit.Auth.API.Dtos;
using NexusFit.Auth.FunctionalTests.Helpers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NexusFit.Auth.FunctionalTests.Endpoints;

[Collection("Test Collection")]
public class AuthControllerLoginTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;
    private readonly Func<Task> _updateApplicationRoles;

    public AuthControllerLoginTests(AuthApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabaseAsync;
        _updateApplicationRoles = apiFactory.UpdateApplicationRoles;
    }

    public async Task InitializeAsync()
    {
        await _updateApplicationRoles();

        var registerDto = new RegisterDto
        {
            Email = "test@email.com",
            Password = "P@ssw0rd",
        };

        var registerRequest = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
        await _client.PostAsync(AuthRoutes.Post.Register, registerRequest);
    }

    public async Task DisposeAsync() => await _resetDatabase();

    [Fact]
    public async Task Login_ShouldReturn_Token()
    {
        var loginDto = new LoginDto
        {
            Email = "test@email.com",
            Password = "P@ssw0rd",
        };

        var request = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(AuthRoutes.Post.Login, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var user = JsonSerializer.Deserialize<UserDto>(content, options);

        user.Should().NotBeNull();
        user?.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LoginWrongPassword_ShouldReturn_Unauthorized()
    {
        var loginDto = new LoginDto
        {
            Email = "test@email.com",
            Password = "password",
        };

        var request = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(AuthRoutes.Post.Login, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LoginUnexisting_ShouldReturn_Unauthorized()
    {
        var loginDto = new LoginDto
        {
            Email = "unexting@email.com",
            Password = "P@ssw0rd",
        };

        var request = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(AuthRoutes.Post.Login, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
