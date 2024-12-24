using Prometheus;

namespace ApiGateway.Common
{
    public class PrometheusMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Counter RecievedRequestsCount = Metrics
            .CreateCounter("apiGateway_jobs_requests_total", "Number of recieved requests");

        public PrometheusMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            RecievedRequestsCount.Inc();
            await _next(context);
        }
    }
}
