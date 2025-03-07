using DAL.Exceptions;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace DAL.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                await HandleExceptionAsync(context, ex);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse();

            switch (ex)
            {
                case NotFoundException notFound:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = notFound.Message;
                    break;

                case ValidationException validation:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = validation.Message;
                    break;

                case DbUpdateConcurrencyException:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response.Message = "Concurrency conflict occurred. The item was modified by another user.";
                    break;

                case DbUpdateException dbEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    if (dbEx.InnerException != null &&
                        dbEx.InnerException.Message.Contains("REFERENCE constraint"))
                    {
                        response.Message = "Bu kayıt başka tablolarda kullanıldığı için silinemez.";
                        response.Detail = "İlişkili kayıtları önce silmeniz gerekmektedir.";
                    }
                    else
                    {
                        response.Message = "Database operation error occurred.";
                        response.Detail = dbEx.InnerException?.Message ?? dbEx.Message;
                    }
                    break;

                case Newtonsoft.Json.JsonException jsonExNewtonsoft:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid JSON format (Newtonsoft).";
                    response.Detail = jsonExNewtonsoft.Message;
                    break;

                case System.Text.Json.JsonException jsonExSystemText:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid JSON format (System.Text.Json).";
                    response.Detail = jsonExSystemText.Message;
                    break;

                case InvalidOperationException invalidOp:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = invalidOp.Message;
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "An internal error occurred.";
                    if (context.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() ?? false)
                    {
                        response.Detail = ex.Message;
                    }
                    break;
            }

            response.Path = context.Request.Path;
            response.Timestamp = DateTime.UtcNow;

            // await context.Response.WriteAsJsonAsync(response);  // bu satırı kaldırın

            var jsonString = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonString);
        }
    }
}
