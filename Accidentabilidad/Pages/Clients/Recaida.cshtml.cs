using Accidentabilidad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Accidentabilidad.Pages.Clients
{
    public class RecaidaModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string InputValue { get; set; }

        public readonly IConfiguration configuration_;

        [BindProperty]
        public Accidente accidente { get; set; }

        [BindProperty]
        public Usuario usuario { get; set; }

        public String errorMessage = "";

        public List<SelectListItem> empleados = new List<SelectListItem>();
        public List<SelectListItem> areas = new List<SelectListItem>();
        public List<SelectListItem> calificaciones = new List<SelectListItem>();
        public List<SelectListItem> atencion = new List<SelectListItem>();

        public Dictionary<string, List<SelectListItem>> calificaciones_ { get; set; }

        public RecaidaModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }
        public void OnGet()
        {
            try
            {
                getCredenciales();
                CargaDatos(InputValue);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Value cannot be null. (Parameter 'value')")
                {
                    errorMessage = "Sesion caducada, es necesario iniciar sesion nuevamente";
                }
            }
        }

        private void getCredenciales()
        {
            var credenciales = JsonConvert.DeserializeObject<Usuario>(HttpContext.Session.GetString("Usuario"));
            usuario = new Usuario() { Correo = credenciales.Correo, Rol = credenciales.Rol };
        }

        private void CargaDatos(string folio)
        {
            empleados = getItems("sp_Cat_empleados_SELECT");
            areas = getItems("SP_Cat_areas_SELECT");
            atencion = getItems("SP_Cat_atencion_SELECT");
            //calificaciones = getItems("SP_Cat_calificacion_SELECT");
            calificaciones_ = getItemsCalificacion();

            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_recaida_SELECT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Folio", folio));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        calificaciones.Add(new SelectListItem
                        {
                            Value = reader.GetInt32("Calificacion_ID").ToString(),
                            Text = reader.GetString("Calificacion")
                        });

                        accidente = new Accidente()
                        {
                            Folio = !reader.IsDBNull(reader.GetOrdinal("Folio")) ? reader.GetString(reader.GetOrdinal("Folio")) : null,
                            Fecha_registro_reporte = reader.GetDateTime("Fecha_registro_reporte"),
                            Nomina = reader.GetString("nomina"),
                            Empleado = reader.GetString("nomina") + " " + reader.GetString("nombre"),
                            Area_ID = reader.GetInt32("Area_ID"),
                            Area = reader.GetString("area"),
                            Atencion_ID = reader.GetInt32("Atencion_ID"),
                            Atencion = reader.GetString("atencion"),
                            Diagnostico = reader.GetString("Diagnostico"),
                            Calificacion_ID = reader.GetInt32("Calificacion_ID"),
                            Calificacion = reader.GetString("Calificacion")
                            //IPP = !reader.IsDBNull(reader.GetOrdinal("IPP")) ? reader.GetString(reader.GetOrdinal("IPP")) : null,
                            //Incapacidad_inicial = reader.GetDateTime("Incapacidad_inicial"),
                            //Inicio_labores = !reader.IsDBNull(reader.GetOrdinal("Inicio_labores")) ? reader.GetDateTime(reader.GetOrdinal("Inicio_labores")) : null,
                            //Dias_subsidiados = reader.GetInt32("Dias_subsidiados"),
                            //Reporta = !reader.IsDBNull(reader.GetOrdinal("Reporta")) ? reader.GetString(reader.GetOrdinal("Reporta")) : null
                        };
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

                            case "SP_Cat_calificacion_SELECT":
                                opciones.Add(new SelectListItem
                                {
                                    Value = reader["id"].ToString(),
                                    Text = reader["Calificacion"].ToString()
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

        private Dictionary<string, List<SelectListItem>> getItemsCalificacion()
        {
            calificaciones_ = new Dictionary<string, List<SelectListItem>>
            {
                {
                    /* IMSS */
                    "1", new List<SelectListItem>
                    {
                        new SelectListItem { Value = "3", Text = "SI DE TRABAJO" },
                        new SelectListItem { Value = "4", Text = "NO DE TRABAJO" },
                        new SelectListItem { Value = "9", Text = "PENDIENTE DE TRABAJO" },
                    }
                },
                {
                    /* INTERNO */
                    "2", new List<SelectListItem>
                    {
                        new SelectListItem { Value = "2", Text = "INTERNO" }
                    }
                },
                {
                    /* TRAYECTO */
                    "4", new List<SelectListItem>
                    {
                        new SelectListItem { Value = "7", Text = "SI DE TRAYECTO" },
                        new SelectListItem { Value = "5", Text = "NO DE TRAYECTO" },
                        new SelectListItem { Value = "10", Text = "PENDIENTE DE TRAYECTO" }
                    }
                }
            };

            return calificaciones_;
        }

        public void OnPost()
        {

            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);

            var Folio = InputValue == null ? "" : InputValue;
            var Incapacidad_inicial = accidente.Incapacidad_inicial == null ? "" : accidente.Incapacidad_inicial.ToString();
            var Inicio_labores = accidente.Inicio_labores == null ? "" : accidente.Inicio_labores.ToString();
            string? Dias_subsidiados = accidente.Dias_subsidiados == null ? "" : accidente.Dias_subsidiados.ToString();
            string IPP = accidente.IPP == null ? "" : accidente.IPP.ToString();
            string Reporta = accidente.Reporta == null ? "" : accidente.Reporta.ToString();
            string Atencion = accidente.Atencion == null ? "" : accidente.Atencion.ToString();

            try
            {
                if (ModelState.IsValid)
                {

                    getCredenciales();

                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_INSERT", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Folio" , Folio));
                    cmd.Parameters.Add(new SqlParameter("@Correo_login", usuario.Correo));
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
                    CargaDatos(InputValue);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

                if (ex.Message == "Value cannot be null. (Parameter 'value')")
                {
                    errorMessage = "Sesion caducada, es necesario cerrar sesion e iniciar sesion nuevamente";
                }
                else
                {
                    CargaDatos(InputValue);
                }
            }
            finally
            {
                con.Close();
            }
        }
    }
}
