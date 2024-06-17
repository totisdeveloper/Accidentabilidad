using Accidentabilidad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace Accidentabilidad.Pages.Historico
{
    public class HistorialModel : PageModel
    {
        public readonly IConfiguration configuration_;

        public HistorialModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }

        public List<Accidente> listAccidentes = new List<Accidente>();
        public String errorMessage = "";
        public void OnGet()
        {

            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_historial_cambios_SELECT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listAccidentes.Add(new Accidente
                        {
                            ID = reader.GetInt32("id"),
                            Folio = reader.GetString("Folio"),
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
