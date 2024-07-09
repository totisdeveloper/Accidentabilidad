using Accidentabilidad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace Accidentabilidad.Pages.Clients
{
    public class IndexModel : PageModel
    {
        public readonly IConfiguration configuration_;

        [BindProperty]
        public Usuario usuario { get; set; }

        public List<Accidente> listAccidentes = new List<Accidente>();

        public String errorMessage = "";


        public IndexModel(IConfiguration configuration)
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
                            Fecha_ocurrencia = !reader.IsDBNull(reader.GetOrdinal("Fecha_ocurrencia")) ? reader.GetDateTime(reader.GetOrdinal("Fecha_ocurrencia")) : null,
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

        public IActionResult OnPost()
        {
            try
            {
                consulta_reporte();
                var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Hoja1");

                    // Headers
                    worksheet.Cells[1, 1].Value = "Fecha";
                    worksheet.Cells[1, 2].Value = "Folio";
                    worksheet.Cells[1, 3].Value = "Nomina";
                    worksheet.Cells[1, 4].Value = "Empleado";
                    worksheet.Cells[1, 5].Value = "Area";
                    worksheet.Cells[1, 6].Value = "Atencion";
                    worksheet.Cells[1, 7].Value = "Diagnostico";
                    worksheet.Cells[1, 8].Value = "Calificacion";
                    worksheet.Cells[1, 9].Value = "IPP";
                    worksheet.Cells[1, 10].Value = "Incapacidad inicial";
                    worksheet.Cells[1, 11].Value = "Inicio labores";
                    worksheet.Cells[1, 12].Value = "Dias subsidiados";
                    worksheet.Cells[1, 13].Value = "Reporta";

                    // Data
                    for (int i = 0; i < listAccidentes.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value  = listAccidentes[i].Fecha_registro_reporte;
                        worksheet.Cells[i + 2, 2].Value  = listAccidentes[i].Folio;
                        worksheet.Cells[i + 2, 3].Value  = listAccidentes[i].Nomina;
                        worksheet.Cells[i + 2, 4].Value  = listAccidentes[i].Empleado;
                        worksheet.Cells[i + 2, 5].Value  = listAccidentes[i].Area;
                        worksheet.Cells[i + 2, 6].Value  = listAccidentes[i].Atencion;
                        worksheet.Cells[i + 2, 7].Value  = listAccidentes[i].Diagnostico;
                        worksheet.Cells[i + 2, 8].Value  = listAccidentes[i].Calificacion;
                        worksheet.Cells[i + 2, 9].Value  = listAccidentes[i].IPP;
                        worksheet.Cells[i + 2, 10].Value = listAccidentes[i].Incapacidad_inicial;
                        worksheet.Cells[i + 2, 11].Value = listAccidentes[i].Inicio_labores;
                        worksheet.Cells[i + 2, 12].Value = listAccidentes[i].Dias_subsidiados;
                        worksheet.Cells[i + 2, 13].Value = listAccidentes[i].Reporta;
                    }
                    package.Save();
                }
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Listado de accidentes.xlsx");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }         
        }
    }
}

