using Accidentabilidad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;

namespace Accidentabilidad.Pages.Clients
{
    public class EditModel : PageModel
    {
        public readonly IConfiguration configuration_;

        [BindProperty]
        public Accidente accidente { get; set; }

        public String errorMessage = "";
        public String successMessage = "";

        public List<SelectListItem> empleados = new List<SelectListItem>();
        public List<SelectListItem> areas = new List<SelectListItem>();
        public List<SelectListItem> calificaciones = new List<SelectListItem>();
        public List<SelectListItem> atencion = new List<SelectListItem>();

        public Dictionary<string, List<SelectListItem>> calificaciones_ { get; set; }

        public EditModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }

        public void OnGet()
        {
            try
            {
                CargaDatos();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Value cannot be null. (Parameter 'value')")
                {
                    errorMessage = "Sesion caducada, es necesario iniciar sesion nuevamente";
                }
            }
        }

        private void CargaDatos()
        {
            var login_usuario = JsonConvert.DeserializeObject<Usuario>(HttpContext.Session.GetString("Usuario")).Correo;

            empleados = getItems("sp_Cat_empleados_SELECT");
            areas = getItems("SP_Cat_areas_SELECT");
            atencion = getItems("SP_Cat_atencion_SELECT");
            //calificaciones = getItems("SP_Cat_calificacion_SELECT");
            calificaciones_ = getItemsCalificacion();

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


                        calificaciones.Add(new SelectListItem
                        {
                            Value = reader.GetInt32("Calificacion_ID").ToString(),
                            Text = reader.GetString("Calificacion")
                        });

                        accidente = new Accidente()
                        {
                            ID = reader.GetInt32("ID"),
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
                            Inicio_labores_date = !reader.IsDBNull(reader.GetOrdinal("Inicio_labores")) ? reader.GetDateTime(reader.GetOrdinal("Inicio_labores")): null,
                            Dias_subsidiados = reader.GetInt32("Dias_subsidiados"),
                            Reporta = !reader.IsDBNull(reader.GetOrdinal("Reporta")) ? reader.GetString(reader.GetOrdinal("Reporta")) : null
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

        public void OnPost()
        {

            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                var login_usuario = JsonConvert.DeserializeObject<Usuario>(HttpContext.Session.GetString("Usuario")).Correo;

                string? Incapacidad_inicial = accidente.Incapacidad_inicial_date == null ? "" : accidente.Incapacidad_inicial_date.ToString();
                string? Inicio_labores = accidente.Inicio_labores_date == null ? "" : accidente.Inicio_labores_date.ToString();
                string? Dias_subsidiados = accidente.Dias_subsidiados == null ? "" : accidente.Dias_subsidiados.ToString();
                string Atencion = accidente.Atencion == null ? "" : accidente.Atencion.ToString();
                string Reporta = accidente.Reporta == null ? "" : accidente.Reporta.ToString();
                string IPP = accidente.IPP == null ? "" : accidente.IPP.ToString();

                if (ModelState.IsValid)
                {
                    SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_UPDATE", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id", accidente.ID));
                    cmd.Parameters.Add(new SqlParameter("@Fecha_registro_reporte", DateTime.Now));
                    cmd.Parameters.Add(new SqlParameter("@Correo_login", login_usuario));
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

                    //foreach (var modelStateKey in ModelState.Keys)
                    //{
                    //    var modelStateVal = ModelState[modelStateKey];
                    //    foreach (var error in modelStateVal.Errors)
                    //    {
                    //        // Aquí puedes manejar o registrar los errores como necesites
                    //        errorMessage  = error.ErrorMessage;
                    //        // Por ejemplo, puedes agregarlos a una lista para mostrar en la página
                    //    }
                    //}


                    CargaDatos();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

                if (ex.Message == "Value cannot be null. (Parameter 'value')")
                {
                    errorMessage = "Sesion caducada, es necesario cerrar sesion e iniciar sesion nuevamente";
                }
            }
            finally
            {
                con.Close();
            }
        }
    }
}
