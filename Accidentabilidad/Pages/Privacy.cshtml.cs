using Accidentabilidad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Accidentabilidad.Pages
{
    public class PrivacyModel : PageModel
    {
        public readonly IConfiguration configuration_;

        [BindProperty]
        public Usuario usuario { get; set; }

        public List<Accidente> listAccidentes = new List<Accidente>();

        public String errorMessage = "";

        public PrivacyModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }

        public void OnGet()
        {
            try
            {
                getCredenciales();
                consulta_reporte();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        private void getCredenciales()
        {
            var credenciales = JsonConvert.DeserializeObject<Usuario>(HttpContext.Session.GetString("Usuario"));
            usuario = new Usuario() { Correo = credenciales.Correo, Rol = credenciales.Rol };
        }

        public void consulta_reporte()
        {
            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_SELECT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Rol", usuario.Rol));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listAccidentes.Add(new Accidente
                        {
                            ID = reader.GetInt32("id"),
                            Folio = !reader.IsDBNull(reader.GetOrdinal("Folio")) ? reader.GetString(reader.GetOrdinal("Folio")) : null,
                            Fecha_registro_reporte = reader.GetDateTime("Fecha_registro_reporte"),
                            Fecha_ocurrencia_text = !reader.IsDBNull(reader.GetOrdinal("Fecha_ocurrencia")) ? reader.GetDateTime(reader.GetOrdinal("Fecha_ocurrencia")).ToShortDateString() : null,
                            Nomina = reader.GetString("nomina"),
                            Empleado = reader.GetString("nombre"),
                            Area = reader.GetString("area"),
                            Atencion = reader.GetString("atencion"),
                            Diagnostico = reader.GetString("Diagnostico"),
                            Calificacion = reader.GetString("Calificacion"),
                            IPP = !reader.IsDBNull(reader.GetOrdinal("IPP")) ? reader.GetString(reader.GetOrdinal("IPP")) : null,
                            Incapacidad_inicial = !reader.IsDBNull(reader.GetOrdinal("Incapacidad_inicial")) ? reader.GetDateTime(reader.GetOrdinal("Incapacidad_inicial")).ToShortDateString() : null,
                            Inicio_labores = !reader.IsDBNull(reader.GetOrdinal("Inicio_labores")) ? reader.GetDateTime(reader.GetOrdinal("Inicio_labores")).ToShortDateString() : null,
                            Dias_subsidiados = !reader.IsDBNull(reader.GetOrdinal("Dias_subsidiados")) ? reader.GetInt32(reader.GetOrdinal("Dias_subsidiados")) : null,
                            Reporta = !reader.IsDBNull(reader.GetOrdinal("Reporta")) ? reader.GetString(reader.GetOrdinal("Reporta")) : null,
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                con.Close();
            }
        }

        public void OnPost()
        {
        }
    }
}