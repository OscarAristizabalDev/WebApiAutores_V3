
namespace WebApiAutores.Middlewares;

public static class LoguearRespuestaHTTPMiddlewareExtensions
{
    public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
    }
}

public class LoguearRespuestaHTTPMiddleware
{
    
    // RequestDelegate sirve para indicar que se van a ejecutar los
    // siguientes middlewares
    private readonly RequestDelegate siguiente;
    private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

    public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente, ILogger<LoguearRespuestaHTTPMiddleware> logger)
    {
        this.siguiente = siguiente;
        this.logger = logger;
    }

    // Invoke o InvokeAsync
    public async Task Invoke(HttpContext contexto)
    {
        using (var ms = new MemoryStream())
        {
            var cuerpoOriginalRespuesta = contexto.Response.Body;
            contexto.Response.Body = ms;
            // Se invocan los siguientes middleware
            await siguiente(contexto);

            ms.Seek(0, SeekOrigin.Begin);
            string respuesta = new StreamReader(ms).ReadToEnd();
            ms.Seek(0, SeekOrigin.Begin);

            await ms.CopyToAsync(cuerpoOriginalRespuesta);
            contexto.Response.Body = cuerpoOriginalRespuesta;

            logger.LogInformation(respuesta);
        }
    }   
}
