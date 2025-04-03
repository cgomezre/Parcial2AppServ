using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parcial2.Models;
using System.Data.Entity;

namespace Parcial2.Data
{


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Prenda> Prendas { get; set; }
        public DbSet<FotoPrenda> FotoPrendas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .HasKey(c => c.Documento);

            modelBuilder.Entity<Prenda>()
                .HasKey(p => p.IdPrenda);

            modelBuilder.Entity<FotoPrenda>()
                .HasKey(f => f.IdFoto);

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Prendas)
                .WithOne(p => p.Cliente)
                .HasForeignKey(p => p.ClienteDocumento);

            modelBuilder.Entity<Prenda>()
                .HasMany(p => p.FotoPrendas)
                .WithOne(f => f.Prenda)
                .HasForeignKey(f => f.IdPrenda);
        }
    }

}