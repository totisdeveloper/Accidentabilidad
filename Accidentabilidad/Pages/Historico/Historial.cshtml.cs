using Accidentabilidad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Accidentabilidad.Pages.Historico
{
    public class HistorialModel : PageModel
    {
        public readonly IConfiguration configuration_;

        [BindProperty]
        public Usuario usuario { get; set; }

        public HistorialModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }

        public List<Accidente> listAccidentes = new List<Accidente>();
        public String errorMessage = "";


        private void getCredenciales()
        {
            var credenciales = JsonConvert.DeserializeObject<Usuario>(HttpContext.Session.GetString("Usuario"));
            usuario = new Usuario() { Correo = credenciales.Correo, Rol = credenciales.Rol };
        }
        public void OnGet()
        {
            getCredenciales();

            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_historial_cambios_SELECT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Rol", usuario.Rol));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listAccidentes.Add(new Accidente
                        {
                            ID = reader.GetInt32("id"),
                            Folio = reader.GetString("Folio"),
                            Fecha_registro_reporte = reader.GetDateTime("Fecha_registro_reporte"),
                            Fecha_ocurrencia_text = !reader.IsDBNull(reader.GetOrdinal("Fecha_ocurrencia")) ? reader.GetDateTime(reader.GetOrdinal("Fecha_ocurrencia")).ToShortDateString() : null,
                            Correo_login = reader.GetString("Correo_login"),
                            Nomina = reader.GetString("nomina"),
                            Empleado = reader.GetString("nombre"),
                            Area = reader.GetString("area"),
                            Atencion = reader.GetString("atencion"),
                            Diagnostico = reader.GetString("Diagnostico"),
                            Calificacion = reader.GetString("Calificacion"),
                            IPP = !reader.IsDBNull(reader.GetOrdinal("IPP")) ? reader.GetString(reader.GetOrdinal("IPP")) : null,
                            Incapacidad_inicial = !reader.IsDBNull(reader.GetOrdinal("Incapacidad_inicial")) ? reader.GetDateTime(reader.GetOrdinal("Incapacidad_inicial")).ToShortDateString() : null,
                            Fecha_modificacion = reader.GetDateTime("Fecha_modificacion"),
                            Inicio_labores = !reader.IsDBNull(reader.GetOrdinal("Inicio_labores")) ? reader.GetDateTime(reader.GetOrdinal("Inicio_labores")).ToShortDateString() : null,
                            Dias_subsidiados = !reader.IsDBNull(reader.GetOrdinal("Dias_subsidiados")) ? reader.GetInt32(reader.GetOrdinal("Dias_subsidiados")) : null,
                            Reporta = !reader.IsDBNull(reader.GetOrdinal("Reporta")) ? reader.GetString(reader.GetOrdinal("Reporta")) : null,
                            Movimiento = !reader.IsDBNull(reader.GetOrdinal("Movimiento")) ? reader.GetString(reader.GetOrdinal("Movimiento")) : null
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
    }
}
