using Accidentabilidad.Datos;
using Accidentabilidad.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Accidentabilidad.Pages.Clients
{
    public class CreateModel : PageModel
    {
        public readonly IConfiguration configuration_;

        public CreateModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }

        public List<SelectListItem> empleados = new List<SelectListItem>();
        public List<SelectListItem> areas = new List<SelectListItem>();
        public List<SelectListItem> calificaciones = new List<SelectListItem>();
        public List<SelectListItem> atencion = new List<SelectListItem>();

        public Dictionary<string, List<SelectListItem>> calificaciones_ { get; set; }

        [BindProperty]
        public Usuario usuario { get; set; }

        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
            try
            {
                getCredenciales();
                carga_listbox();
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

        private void carga_listbox()
        {
            empleados = getItems("sp_Cat_empleados_SELECT");
            areas = getItems("SP_Cat_areas_SELECT");
            //calificaciones = getItems("SP_Cat_calificacion_SELECT");
            atencion = getItems("SP_Cat_atencion_SELECT");
            calificaciones_ = getItemsCalificacion();
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
                                opciones.Add(new SelectListItem
                                {
                                    Value = reader["id"].ToString(),
                                    Text = reader["area"].ToString()
                                });
                                break;

                            case "sp_Cat_empleados_SELECT":
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

        [BindProperty]
        public Accidente accidente { get; set; }
        public void OnPost()
        {
            getCredenciales();

            string connectionString = configuration_.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(connectionString);

            var Incapacidad_inicial = accidente.Incapacidad_inicial == null ? "" : accidente.Incapacidad_inicial.ToString();
            var Inicio_labores = accidente.Inicio_labores == null ? "" : accidente.Inicio_labores.ToString();
            string? Dias_subsidiados = accidente.Dias_subsidiados == null ? "" : accidente.Dias_subsidiados.ToString();
            string IPP = accidente.IPP == null ? "" : accidente.IPP.ToString();
            string Reporta = accidente.Reporta == null ? "" : accidente.Reporta.ToString();

            Dias_subsidiados = obtenerDias(Incapacidad_inicial, Inicio_labores);

            try
            {
                if (ModelState.IsValid)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_Rep_accidentes_INSERT", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Folio", string.Empty));
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
                    carga_listbox();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                carga_listbox();
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
