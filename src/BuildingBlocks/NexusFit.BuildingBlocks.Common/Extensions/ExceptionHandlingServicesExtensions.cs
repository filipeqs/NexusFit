using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NexusFit.BuildingBlocks.Common.Models;

namespace NexusFit.BuildingBlocks.Common.Extensions
{
    public static class ExceptionHandlingServicesExtensions
    {
        public static IServiceCollection AddExceptionHandlingServices(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(q => q.Value.Errors)
                        .Select(q => q.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}