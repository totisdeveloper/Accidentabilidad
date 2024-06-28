using System.ComponentModel.DataAnnotations;

namespace Accidentabilidad.Models
{
    public class Calificacion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
    }
}
