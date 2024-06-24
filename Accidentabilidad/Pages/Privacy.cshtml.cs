using Accidentabilidad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;

namespace Accidentabilidad.Pages
{
    public class PrivacyModel : PageModel
    {
        public List<SelectListItem> empleados = new List<SelectListItem>();
        public List<SelectListItem> areas = new List<SelectListItem>();
        public List<SelectListItem> calificaciones = new List<SelectListItem>();
        public List<SelectListItem> atencion = new List<SelectListItem>();

        [BindProperty]
        public Accidente accidente { get; set; }

        public readonly IConfiguration configuration_;

        public PrivacyModel(IConfiguration configuration)
        {
            configuration_ = configuration;
        }
        public void OnGet()
        {
            try
            {
                carga_listbox();
            }
            catch (Exception ex)
            {


            }
        }


        private void carga_listbox()
        {
            empleados = getItems("sp_Cat_empleados_SELECT");
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
    }
}