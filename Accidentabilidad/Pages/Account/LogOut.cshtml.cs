using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Reflection;
using System.Security.Policy;
using System;

namespace Accidentabilidad.Pages.Account
{
    public class LogOutModel : PageModel
    {
        public void OnGet()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            // Redirigir a la página de inicio o a otra página deseada
            Response.Redirect("/");
        }

        public void OnPost() 
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            // Redirigir a la página de inicio o a otra página deseada
            Response.Redirect("/");
        }
    }
}


