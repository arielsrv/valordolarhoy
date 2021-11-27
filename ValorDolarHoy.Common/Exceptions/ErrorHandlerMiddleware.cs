using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ValorDolarHoy.Common.Exceptions
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
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
                httpResponse.ContentType = "application/json";

                httpResponse.StatusCode = error switch
                {
                    ApiBadRequestException => (int)HttpStatusCode.BadRequest,
                    ApiNotFoundException => (int)HttpStatusCode.NotFound,

                    _ => (int)HttpStatusCode.InternalServerError
                };

                string result = JsonConvert.SerializeObject(new
                {
                    code = httpResponse.StatusCode,
                    type = error.GetType().Name,
                    message = error.Message,
                    detail = error.StackTrace
                });

                await httpResponse.WriteAsync(result);
            }
        }
    }
}