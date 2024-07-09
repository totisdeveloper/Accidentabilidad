using System.ComponentModel.DataAnnotations;

namespace Accidentabilidad.Models
{
    public class Accidente
    {
        public int ID { get; set; }
        public string? Folio { get; set; }

        public DateTime Fecha_registro_reporte { get; set; }

        [Required(ErrorMessage = "El campo Fecha ocurrencia es obligatorio")]
        public DateTime?Fecha_ocurrencia { get; set; }
        public string? Correo_login { get; set; }

        [Required(ErrorMessage = "El campo Nomina es obligatorio")]
        public string Nomina { get; set; }

        public string? Empleado { get; set; }

        [Required(ErrorMessage = "El campo Area es obligatorio")]
        public int? Area_ID { get; set; }

        public string? Area { get; set; }

        [Required(ErrorMessage = "El campo Atencion es obligatorio")]
        public int? Atencion_ID { get; set; }

        public string? Atencion { get; set; }

        [Required(ErrorMessage = "El campo Diagnostico es obligatorio")]
        public string? Diagnostico { get; set; }

        [Required(ErrorMessage = "El campo calificacion es obligatorio")]
        public int? Calificacion_ID { get; set; }

        public string? Calificacion { get; set; }

        //[Required(ErrorMessage = "El campo incapacidad inicial es obligatorio")]
        //public DateTime? Incapacidad_inicial { get; set; }
        public string? Incapacidad_inicial { get; set; }

        public DateTime? Incapacidad_inicial_date { get; set; }

        //public DateTime? Inicio_labores { get; set; }
        public string? Inicio_labores { get; set; }
        public DateTime? Inicio_labores_date { get; set; }

        public int? Dias_subsidiados { get; set; }

        public string? Reporta { get; set; }
        public DateTime? Fecha_modificacion { get; set; }

        public string? IPP { get; set; }
    }
}
