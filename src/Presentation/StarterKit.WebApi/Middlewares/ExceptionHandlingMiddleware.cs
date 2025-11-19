using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Exceptions;
using System.Net;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace StarterKit.WebApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ILocalizationService _localizationService;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger,
            ILocalizationService localizationService)
        {
            _next = next;
            _logger = logger;
            _localizationService = localizationService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured while processing the request.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;
            var problemDetails = new ProblemDetails();

            // Accept-Language header
            string lang = context.Request.Headers["Accept-Language"].ToString().ToLower();
            if (string.IsNullOrEmpty(lang) || !(lang == "az" || lang == "en" || lang == "ru"))
                lang = "az";

            switch (ex)
            {
                case ApplicationException:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Detail = ex.Message;
                    problemDetails.Title = "Application Error";
                    break;
                case KeyNotFoundException:
                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails.Detail = ex.Message;
                    problemDetails.Title = "Not Found";
                    break;
                case UnAuthorizedException exc:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Detail = exc.Message;
                    problemDetails.Title = "Unauthorized";
                    break;
                case UniqueException exc:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails.Detail = exc.Message;
                    problemDetails.Title = "Unique property";
                    break;
                case UserAlreadyActivatedException exc:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    problemDetails.Detail = exc.Message;
                    problemDetails.Title = "Already Activated";
                    break;
                case UserAlreadyExistedException exc:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    problemDetails.Detail = exc.Message;
                    problemDetails.Title = "User Already Existed";
                    break;
                case ExtensionException exc:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails.Detail = exc.Message;
                    problemDetails.Title = "Extension Error";
                    break;
                case BadRequestException exc:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails.Detail = exc.Message;
                    problemDetails.Title = "Bad Request Error";
                    break;
                case ValidationException exc:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableContent;

                    var localizedErrors = new Dictionary<string, string[]>();
                    foreach (var error in exc.Errors)
                    {
                        localizedErrors[error.Key] = error.Value
                            .Select(msgKey => _localizationService.GetMessage(msgKey, lang))
                            .ToArray();
                    }

                    problemDetails = new ValidationProblemDetails(localizedErrors)
                    {
                        Title = _localizationService.GetMessage("ValidationError", lang)
                    };
                    problemDetails.Extensions.Add("errors", localizedErrors);
                    problemDetails.Title = "Validation error";
                    break;
                case ForbiddenException exc:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    problemDetails.Detail = exc.Message;
                    problemDetails.Title = "Forbidden";
                    break;
                case LockedException exc:
                    response.StatusCode = (int)HttpStatusCode.Locked;
                    problemDetails.Detail = exc.Message;
                    problemDetails.Title = "Locked";
                    break;


                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Detail = ex.Message;
                    problemDetails.Title = "Server Error";
                    break;
            }

            var extensionErrors = string.Join(", ", problemDetails.Extensions.Select(e => $"{e.Key}: {JsonSerializer.Serialize(e.Value)}"));
            _logger.LogError($"Exception caught: {problemDetails.Title}, Status Code: {response.StatusCode}, Details: {problemDetails.Detail}, Extensions: {extensionErrors}");


            // Tərcümə edilmiş mesaj
            if (!(ex is ValidationException))
                problemDetails.Detail = _localizationService.GetMessage(problemDetails.Detail, lang);

            // Build final unified JSON response for FE
            var responseBody = new Dictionary<string, object?>
            {
                { "status", response.StatusCode },
                { "title", problemDetails.Title },
                { "message", problemDetails.Detail }
            };

            if (problemDetails is ValidationProblemDetails vpd)
                responseBody.Add("errors", vpd.Errors);

            var result = JsonSerializer.Serialize(responseBody);
            await context.Response.WriteAsync(result);


            //var result = JsonSerializer.Serialize(problemDetails);
            //await context.Response.WriteAsync(result);
        }
    }
}
