
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace StoryApp.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly RequestDelegate next;
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try 
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var message = $"An error occurred while processing {context.Request.Method} {context.Request.GetDisplayUrl()}";
                logger.LogError(ex, message);

                var problemDetails = new ProblemDetails
                {
                    Type = "Status500InternalServerError",
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred. Please try again later.",
                    Instance = context.Request.GetDisplayUrl()
                };

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
