using Accidentabilidad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Accidentabilidad.Pages.Clients
{
    public class EliminarModel : PageModel
    {
        public readonly IConfiguration configuration_;

        [BindProperty]
        public Usuario usuario { get; set; }

        public EliminarModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }
        public void OnGet()
        {
            getCredenciales();

            int id = Convert.ToInt32(Request.Query["ID"]);

            string connectionString = configuration_.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_DELETE", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ID", id));
                cmd.Parameters.Add(new SqlParameter("@Correo_login", usuario.Correo));
                cmd.ExecuteNonQuery();

                Response.Redirect("/Clients/Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
        }

        private void getCredenciales()
        {
            var credenciales = JsonConvert.DeserializeObject<Usuario>(HttpContext.Session.GetString("Usuario"));
            usuario = new Usuario() { Correo = credenciales.Correo, Rol = credenciales.Rol };
        }

        public void OnPost()
        { 
        }
    }
}
