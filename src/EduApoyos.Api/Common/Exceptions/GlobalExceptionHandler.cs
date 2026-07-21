using System.Diagnostics;
using EduApoyos.Application.Common.Exceptions;
using EduApoyos.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EduApoyos.Api.Common.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (
            statusCode,
            title,
            detail,
            type) = ObtenerInformacionError(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Error no controlado. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                "Error controlado {StatusCode}: {Message}",
                statusCode,
                exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] =
            Activity.Current?.Id ??
            httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }

    private static (
        int StatusCode,
        string Title,
        string Detail,
        string Type)
        ObtenerInformacionError(Exception exception)
    {
        return exception switch
        {
            AccesoDenegadoException => (
             StatusCodes.Status403Forbidden,
             "Acceso denegado",
             exception.Message,
             "https://tools.ietf.org/html/rfc9110#section-15.5.4"),

            NoAutorizadoException => (
                StatusCodes.Status401Unauthorized,
                "No autorizado",
                exception.Message,
                "https://tools.ietf.org/html/rfc9110#section-15.5.2"),

            ConflictoException => (
                StatusCodes.Status409Conflict,
                "Conflicto",
                exception.Message,
                "https://tools.ietf.org/html/rfc9110#section-15.5.10"),

            ReglaNegocioException => (
                StatusCodes.Status422UnprocessableEntity,
                "Regla de negocio no satisfecha",
                exception.Message,
                "https://tools.ietf.org/html/rfc9110#section-15.5.21"),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                exception.Message,
                "https://tools.ietf.org/html/rfc9110#section-15.5.5"),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                exception.Message,
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado al procesar la solicitud.",
                "https://tools.ietf.org/html/rfc9110#section-15.6.1")
        };
    }
}