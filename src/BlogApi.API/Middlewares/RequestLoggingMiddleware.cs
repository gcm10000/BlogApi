using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlogApi.API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var request = context.Request;
            var method = request.Method;
            var path = request.Path;
            var queryString = request.QueryString.ToString();
            var headers = request.Headers;

            string body = "";

            // Habilita o buffer do body para leitura
            context.Request.EnableBuffering();

            if (request.ContentLength > 0 && request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                body = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin);
            }

            await _next(context);

            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;

            _logger.LogInformation("HTTP {Method} {Path}{QueryString} responded {StatusCode} in {ElapsedMilliseconds}ms\nHeaders: {Headers}\n\n\nBody: {Body}",
                method,
                path,
                queryString,
                statusCode,
                stopwatch.ElapsedMilliseconds,
                headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                body
            );
        }
    }
}
