using System.Data.Entity;

namespace Parcial2.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=DefaultConnection")
        {
            // Puedes poner aquí configuraciones adicionales si quieres
        }

        // Representa la tabla Cliente
        public DbSet<Cliente> Clientes { get; set; }

        // Representa la tabla Prenda
        public DbSet<Prenda> Prendas { get; set; }

        // Representa la tabla FotoPrenda
        public DbSet<FotoPrenda> FotosPrendas { get; set; }
    }
}
