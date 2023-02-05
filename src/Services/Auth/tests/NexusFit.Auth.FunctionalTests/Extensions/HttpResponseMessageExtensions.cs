using System.Text.Json;

namespace NexusFit.Auth.FunctionalTests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async  Task<T> SerializeResponse<T>(this HttpResponseMessage response)
    {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<T>(jsonResponse, options);
    }
}
