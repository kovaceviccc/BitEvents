using BitEvents.Api.Contracts.Responses;
using FluentValidation;
using BitEvents.Api.Exceptions;

namespace BitEvents.Api.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _request;

    public ExceptionMiddleware(RequestDelegate request, ILogger<ExceptionMiddleware> logger)
    {
        _request = request;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = 400;
            var messages = exception.Errors.Select(x => x.ErrorMessage).ToList();
            var validationFailureResponse = new ValidationFailureResponse
            {
                Errors = messages
            };
            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
        catch (NotFoundException exception)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(exception.Message);
        }
        catch (InvalidInputException exception)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(exception.Message);
        }
        catch (DatabaseException exception)
        {
            _logger.LogError(exception.Message, exception.InnerException, exception.StackTrace);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message, exception.StackTrace);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(exception.Message);
        }
    }
}