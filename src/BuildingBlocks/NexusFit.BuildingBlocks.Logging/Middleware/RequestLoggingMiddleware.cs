using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NexusFit.BuildingBlocks.Logging.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {

        _logger.LogInformation(
            "Start: Request {method} {url} => {statusCode}",
            context.Request?.Method,
            context.Request?.Path.Value,
            context.Response?.StatusCode);

        await _next(context);

        _logger.LogInformation(
            "End: Request {method} {url} => {statusCode}",
            context.Request?.Method,
            context.Request?.Path.Value,
            context.Response?.StatusCode);
    }
}
