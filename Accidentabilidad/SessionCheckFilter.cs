using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Accidentabilidad
{
    public class SessionCheckFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetString("UserId");

            // Verificar si el usuario está autenticado según tus requisitos
            if (string.IsNullOrEmpty(userId))
            {
                // Redirigir a la página de inicio de sesión si no hay sesión activa
                context.Result = new RedirectToPageResult("/Account/Login");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No es necesario realizar ninguna acción después de ejecutar el método de acción
        }
    }
}
