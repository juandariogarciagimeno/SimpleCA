
using Microsoft.AspNetCore.Mvc;

namespace SimpleCA.API.Global
{
    public class ErrorHandling
    {
        private readonly RequestDelegate _next;

        public ErrorHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails()
                {
                    Type = "",
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing your request.",
                    Status = 500,
                    Extensions = new Dictionary<string, object?>()
                    {
                        { "exception", ex.Message },
                    },
                };

                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
