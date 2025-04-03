using System.Data.Entity;
using Parcial2.Models;

namespace Parcial2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection") { }

        public DbSet<Cliente_Models> Clientes { get; set; }
        public DbSet<Prenda_Models> Prendas { get; set; }
        public DbSet<FotoPrenda> FotoPrendas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente_Models>()
                .HasKey(c => c.Documento)
                .ToTable("Cliente");

            modelBuilder.Entity<Prenda_Models>()
                .HasKey(p => p.IdPrenda)
                .ToTable("Prenda");

            modelBuilder.Entity<FotoPrenda>()
                .HasKey(f => f.idFoto)
                .ToTable("FotoPrenda");

            modelBuilder.Entity<Cliente_Models>()
                .HasMany(c => c.Prendas)
                .WithRequired()
                .HasForeignKey(p => p.Cliente);

            modelBuilder.Entity<Prenda_Models>()
                .HasMany(p => p.Fotos)
                .WithRequired()
                .HasForeignKey(f => f.idPrenda);
        }
    }
}