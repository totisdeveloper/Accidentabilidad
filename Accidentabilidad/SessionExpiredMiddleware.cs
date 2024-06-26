namespace Accidentabilidad
{
    public class SessionExpiredMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionExpiredMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();

            // Excluir la página de inicio de sesión y la página de sesión expirada
            if (path != "/" && path != "/sessionexpired")
            {
                if (!context.Session.IsAvailable || !context.Session.Keys.Contains("Usuario"))
                {
                    if (path == "/account/logout")
                        context.Response.Redirect("/");
                    else
                        context.Response.Redirect("/SessionExpired");

                    return;
                }
            }

            await _next(context);
        }
    }
    public static class SessionExpiredMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionExpiredMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionExpiredMiddleware>();
        }
    }
}
