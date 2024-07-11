using Accidentabilidad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services;

namespace Accidentabilidad.Pages.Clients
{
    public class EditModel : PageModel
    {
        public readonly IConfiguration configuration_;

        [BindProperty]
        public Accidente accidente { get; set; }

        [BindProperty]
        public Usuario usuario { get; set; }

        public String errorMessage = "";
        public String successMessage = "";

        public List<SelectListItem> empleados = new List<SelectListItem>();
        public List<SelectListItem> areas = new List<SelectListItem>();

        public List<SelectListItem> atencion = new List<SelectListItem>();
        public List<Calificacion> Cities { get; set; }

        [BindProperty]
        public int SelectedCountryId { get; set; }

        [BindProperty]
        public int SelectedCityId { get; set; }

        [BindProperty]
        public int SelectedCityId_ { get; set; }


        public EditModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }

        public void OnGet()
        {
            try
            {
                getCredenciales();
                CargaDatos();
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

        private void CargaDatos()
        {
            empleados = getItems("sp_Cat_empleados_SELECT");
            areas = getItems("SP_Cat_areas_SELECT");
            atencion = getItems("SP_Cat_atencion_SELECT");
            carga_calificacion();

            int id = Convert.ToInt32(Request.Query["ID"]);

            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_edit_SELECT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ID", id));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        accidente = new Accidente()
                        {
                            ID = reader.GetInt32("ID"),
                            Fecha_ocurrencia = !reader.IsDBNull(reader.GetOrdinal("Fecha_ocurrencia")) ? reader.GetDateTime(reader.GetOrdinal("Fecha_ocurrencia")) : null,
                            Fecha_registro_reporte = reader.GetDateTime("Fecha_registro_reporte"),
                            Nomina = reader.GetString("nomina"),
                            Empleado = reader.GetString("nomina") + " " + reader.GetString("nombre"),
                            Area_ID = reader.GetInt32("Area_ID"),
                            Area = reader.GetString("area"),
                            Atencion_ID = reader.GetInt32("Atencion_ID"),
                            Atencion = reader.GetString("atencion"),
                            Diagnostico = reader.GetString("Diagnostico"),
                            Calificacion_ID = reader.GetInt32("Calificacion_ID"),
                            Calificacion = reader.GetString("Calificacion"),
                            IPP = !reader.IsDBNull(reader.GetOrdinal("IPP")) ? reader.GetString(reader.GetOrdinal("IPP")) : null,
                            Incapacidad_inicial_date = !reader.IsDBNull(reader.GetOrdinal("Incapacidad_inicial")) ? reader.GetDateTime(reader.GetOrdinal("Incapacidad_inicial")) : null,
                            Inicio_labores_date = !reader.IsDBNull(reader.GetOrdinal("Inicio_labores")) ? reader.GetDateTime(reader.GetOrdinal("Inicio_labores")) : null,
                            Dias_subsidiados = reader.GetInt32("Dias_subsidiados"),
                            Reporta = !reader.IsDBNull(reader.GetOrdinal("Reporta")) ? reader.GetString(reader.GetOrdinal("Reporta")) : null
                        };

                        SelectedCityId_ = reader.GetInt32("Calificacion_ID");
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

        private void carga_calificacion()
        {
            Cities = new List<Calificacion>();

            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Cat_calificacion_SELECT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cities.Add(new Calificacion
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ID")),
                            Name = reader.GetString(reader.GetOrdinal("Calificacion")),
                            CountryId = reader.GetInt32(reader.GetOrdinal("Atencion_ID"))
                        });
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
        }

        private List<SelectListItem> getItems(string storeName)
        {
            List<SelectListItem> opciones = new List<SelectListItem>();

            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(storeName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        switch (storeName)
                        {
                            case "SP_Cat_areas_SELECT":
                                var id = reader["id"].ToString();
                                var area = reader["area"].ToString();

                                opciones.Add(new SelectListItem
                                {
                                    Value = reader["id"].ToString(),
                                    Text = reader["area"].ToString()
                                });
                                break;

                            case "sp_Cat_empleados_SELECT":
                                var nomina = reader["nomina"].ToString();
                                var nombre = reader["Nombre"].ToString();
                                opciones.Add(new SelectListItem
                                {
                                    Value = reader["nomina"].ToString(),
                                    Text = reader["nomina"].ToString() + "  " + reader["Nombre"].ToString()
                                });
                                break;

                            case "SP_Cat_atencion_SELECT":

                                var id_ = reader["id"].ToString();
                                var atencion = reader["Atencion"].ToString();
                                opciones.Add(new SelectListItem
                                {
                                    Value = reader["id"].ToString(),
                                    Text = reader["Atencion"].ToString()
                                });
                                break;

                            default:
                                break;
                        }
                    }
                    reader.Close();
                }
                return opciones;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                return opciones;
            }
            finally
            {
                con.Close();
            }
        }

        public void OnPost()
        {
            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                var login_usuario = JsonConvert.DeserializeObject<Usuario>(HttpContext.Session.GetString("Usuario"));

                var fecha_ocurrencia = accidente.Fecha_ocurrencia == null ? "" : accidente.Fecha_ocurrencia.ToString();
                string? Incapacidad_inicial = accidente.Incapacidad_inicial_date == null ? "" : accidente.Incapacidad_inicial_date.ToString();
                string? Inicio_labores = accidente.Inicio_labores_date == null ? "" : accidente.Inicio_labores_date.ToString();
                string? Dias_subsidiados = accidente.Dias_subsidiados == null ? "" : accidente.Dias_subsidiados.ToString();
                string Atencion = accidente.Atencion_ID == null ? "" : accidente.Atencion_ID.ToString();
                string Reporta = accidente.Reporta == null ? "" : accidente.Reporta.ToString();
                string IPP = accidente.IPP == null ? "" : accidente.IPP.ToString();


                if (login_usuario.Rol != "1" && Dias_subsidiados == "")
                {
                    Dias_subsidiados = obtenerDias(Incapacidad_inicial, Inicio_labores);
                }

                if (ModelState.IsValid)
                {
                    SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_UPDATE", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id", accidente.ID));
                    cmd.Parameters.Add(new SqlParameter("@Fecha_ocurrencia", fecha_ocurrencia));
                    cmd.Parameters.Add(new SqlParameter("@Fecha_registro_reporte", accidente.Fecha_registro_reporte));
                    cmd.Parameters.Add(new SqlParameter("@Correo_login", login_usuario.Correo));
                    cmd.Parameters.Add(new SqlParameter("@Nomina", accidente.Nomina));
                    cmd.Parameters.Add(new SqlParameter("@Area_ID", accidente.Area_ID));
                    cmd.Parameters.Add(new SqlParameter("@Atencion_ID", accidente.Atencion_ID));
                    cmd.Parameters.Add(new SqlParameter("@Diagnostico", accidente.Diagnostico));
                    cmd.Parameters.Add(new SqlParameter("@Calificacion_ID", accidente.Calificacion_ID));
                    cmd.Parameters.Add(new SqlParameter("@IPP", IPP));
                    cmd.Parameters.Add(new SqlParameter("@Incapacidad_inicial", Incapacidad_inicial));
                    cmd.Parameters.Add(new SqlParameter("@Inicio_labores", Inicio_labores));
                    cmd.Parameters.Add(new SqlParameter("@Dias_subsidiados", Dias_subsidiados));
                    cmd.Parameters.Add(new SqlParameter("@Reporta", Reporta));
                    cmd.ExecuteNonQuery();

                    Response.Redirect("/Clients/Index");
                }
                else
                {
                    CargaDatos();
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

        private string obtenerDias(string fecha_Inicial, string fecha_Final)
        {
            int dias = 0;

            if (fecha_Inicial != "" && fecha_Final != "")
            {
                DateTime fechaInicial = Convert.ToDateTime(fecha_Inicial);
                DateTime fechaFinal = Convert.ToDateTime(fecha_Final);

                // Calcular la diferencia de días
                TimeSpan diferencia = fechaFinal.Subtract(fechaInicial);

                // Obtener el número de días
                dias = diferencia.Days;
            }
            return dias.ToString();
        }
    }
}
