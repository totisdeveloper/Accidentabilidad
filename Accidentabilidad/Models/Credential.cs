using System.ComponentModel.DataAnnotations;

namespace Accidentabilidad.Models
{
    public class Credential
    {
        [Required(ErrorMessage = "El campo Correo es obligatorio")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es obligatorio")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }
    }
}
