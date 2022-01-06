using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ValorDolarHoy.Core.Common.Exceptions;

namespace ValorDolarHoy.Core.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public class ErrorModel
    {
        public int Code { get; }
        public string Type { get; }
        public string Message { get; }
        public string? Detail { get; }

        public ErrorModel(int code, string type, string message, string? detail)
        {
            this.Code = code;
            this.Type = type;
            this.Message = message;
            this.Detail = detail;
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this.next(context);
        }
        catch (Exception error)
        {
            HttpResponse httpResponse = context.Response;
            httpResponse.ContentType = MediaTypeNames.Application.Json;
            httpResponse.StatusCode = error switch
            {
                ApiBadRequestException => (int)HttpStatusCode.BadRequest,
                ApiNotFoundException => (int)HttpStatusCode.NotFound,

                _ => (int)HttpStatusCode.InternalServerError
            };

            ErrorModel errorModel = new(
                httpResponse.StatusCode,
                error.GetType().Name,
                $"{error.Data["HttpClient"]}. {error.Message}",
                error.StackTrace);

            string result = JsonConvert.SerializeObject(errorModel);

            await httpResponse.WriteAsync(result);
        }
    }
}