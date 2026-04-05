using System.Diagnostics;
using System.Text;

public class RequestLogger
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLogger> _logger;

    public RequestLogger(RequestDelegate next, ILogger<RequestLogger> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Start a timer
        var stopWatch = Stopwatch.StartNew();

        // 2. Log the Incoming Request
        _logger.LogInformation("--- Incoming Request: {Method} {Path} ---", 
            context.Request.Method, context.Request.Path);

        // 3. Let the request continue to the Controller
        await _next(context); //request moves to the next mw or to the controller if this is the last mw

        // 4. Request is finished! Log the outcome
        stopWatch.Stop();
        var statusCode = context.Response.StatusCode;
        
        _logger.LogInformation("--- Finished Request: {Method} {Path} responded {Status} in {Elapsed}ms ---",
            context.Request.Method, context.Request.Path, statusCode, stopWatch.ElapsedMilliseconds);
    }
}