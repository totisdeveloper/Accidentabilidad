using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.DirectoryServices.AccountManagement;
using Accidentabilidad.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Data;

namespace Accidentabilidad.Pages.Account
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class LoginModel : PageModel
    {

        public readonly IConfiguration configuration_;

        public LoginModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }

        [BindProperty]
        public Credential Credential { get; set; }

        [BindProperty]
        public string nombre { get; set; }

        public String errorMessage = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (Credential.Correo != null && Credential.Contraseña != null)
            {
                try
                {
                    if (UserAuthentication(Credential.Correo, Credential.Contraseña))
                    {
                        var usuario = new Usuario()
                        {
                            Correo = Credential.Correo,
                            Rol = get_user_role(Credential.Correo)
                        };
                        HttpContext.Session.SetString("Usuario", JsonConvert.SerializeObject(usuario));
                        return RedirectToPage("/Clients/Index");
                    }
                    else
                        throw new Exception("Clave y/o contraseña incorrectos");
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
            return Page();
        }
        public bool UserAuthentication(string Correo, string Contraseña)
        {
            var LDAP_PATH = Environment.GetEnvironmentVariable("LDAP_PATH");
            var dominio = Environment.GetEnvironmentVariable("DOMINIO");
            //use the users credentials for the query
            DirectoryEntry root = new DirectoryEntry(
                LDAP_PATH,
                dominio + Correo + "",
                Contraseña
                );

            //query for the username provided
            DirectorySearcher searcher = new DirectorySearcher(
                root,
                "(sAMAccountName=" + dominio + Correo + "" + ")"
                );

            //a success means the password was right
            bool success = false;
            try
            {
                searcher.FindOne();
                success = true;
            }
            catch
            {
                success = false;
            }
            return success;
        }


        private string get_user_role(string correo)
        {
            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);

            con.Open();
            SqlCommand cmd = new SqlCommand("SP_select_rol_cat_roles", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Correo", correo));

            var rol = cmd.ExecuteScalar().ToString();

            return rol == null ? "" : rol;
        }
    }
}
