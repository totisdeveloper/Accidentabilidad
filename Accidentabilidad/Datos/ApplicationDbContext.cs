using Accidentabilidad.Models;
using Microsoft.EntityFrameworkCore;

namespace Accidentabilidad.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
