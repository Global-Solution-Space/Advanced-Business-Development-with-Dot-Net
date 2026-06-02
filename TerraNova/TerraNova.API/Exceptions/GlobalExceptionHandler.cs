using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using TerraNova.Domain.Exceptions;

namespace TerraNova.API.Exceptions;

/// <summary>
/// Intercepta todas as exceções não tratadas e devolve um <c>ProblemDetails</c> padronizado.
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exceção não tratada: {Message}", exception.Message);
 
        var (statusCode, title, detail) = MapException(exception, environment);
 
        httpContext.Response.StatusCode  = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
 
        var problem = new ProblemDetails
        {
            Type     = "about:blank",
            Title    = title,
            Status   = statusCode,
            Detail   = detail,
            Instance = httpContext.Request.Path
        };
 
        if (environment.IsDevelopment())
            problem.Extensions["traceId"] = httpContext.TraceIdentifier;
 
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
 
    private static (int StatusCode, string Title, string? Detail) MapException(
        Exception exception,
        IHostEnvironment environment) => exception switch
    {
        ArgumentNullException e      => (StatusCodes.Status400BadRequest,  "Requisição inválida",                  e.Message),
        ArgumentException e          => (StatusCodes.Status400BadRequest,  "Requisição inválida",                  e.Message),
        DomainException e            => (StatusCodes.Status400BadRequest,  "Regra de negócio violada",             e.Message),
        InvalidOperationException e  => (StatusCodes.Status400BadRequest,  "Não foi possível concluir a operação", e.Message),
        KeyNotFoundException e       => (StatusCodes.Status404NotFound,    "Recurso não encontrado",               e.Message),
        UnauthorizedAccessException e=> (StatusCodes.Status401Unauthorized,"Não autorizado",                       e.Message),
        OracleException e            => (StatusCodes.Status502BadGateway,  "Banco de dados indisponível",          e.Message),
        _                            => MapUnhandled(environment, exception)
    };
 
    private static (int StatusCode, string Title, string? Detail) MapUnhandled(
        IHostEnvironment environment,
        Exception exception) =>
    (
        StatusCodes.Status500InternalServerError,
        "Erro interno do servidor",
        environment.IsDevelopment()
            ? exception.ToString()
            : "Ocorreu um erro inesperado. Tente novamente mais tarde."
    );
}