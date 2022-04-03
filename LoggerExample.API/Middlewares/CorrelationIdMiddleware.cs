using Microsoft.Extensions.Primitives;

namespace LoggerExample.API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        public const string CorrelationIdHeaderKey = "X-Correlation-ID";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;
        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string correlationId = String.Empty;

            if (!httpContext.Request.Headers.TryGetValue(
                    CorrelationIdHeaderKey, out StringValues values))
            {
                correlationId = Guid.NewGuid().ToString();
                httpContext.Request.Headers.Add(CorrelationIdHeaderKey,
                    correlationId);
            }
            else
            {
                correlationId = values.First();
            }

            var logger = httpContext.RequestServices.GetRequiredService<ILogger<CorrelationIdMiddleware>>();

            using (logger.BeginScope("{@CorrelationId}", correlationId))
            {
                await _next.Invoke(httpContext);
            }



        }
    }
}
