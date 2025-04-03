using Parcial2.Models;
using Parcial2.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Web.Http.Results;

namespace Parcial2.Controllers
{
    [RoutePrefix("api/prendas")]
    public class PrendasController : ApiController
    {
        private readonly AppDbContext _context;

        public PrendasController()
        {
            _context = new AppDbContext();
        }

        // GET: api/prendas/cliente/{documento}
        [HttpGet]
        [Route("cliente/{documento}")]
        public async Task<IHttpActionResult> ObtenerPrendasPorCliente(string documento)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Prendas.Select(p => p.FotoPrendas))
                .FirstOrDefaultAsync(c => c.Documento == documento);

            if (cliente == null)
                return NotFound();

            var resultado = new
            {
                cliente.Documento,
                cliente.Nombre,
                cliente.Email,
                cliente.Celular,
                Prendas = cliente.Prendas.Select(p => new
                {
                    p.IdPrenda,
                    p.TipoPrenda,
                    p.Descripcion,
                    p.Valor,
                    Imagenes = p.FotoPrendas.Select(f => f.Prenda).ToList()
                }).ToList()
            };

            return Ok(resultado);
        }

        // POST: api/prendas/agregar
        [HttpPost]
        [Route("agregar")]
        public async Task<IHttpActionResult> AgregarPrenda([FromBody] Prenda_Models prenda)
        {
            if (prenda == null || string.IsNullOrEmpty(prenda.Cliente))
                return BadRequest("Los datos de la prenda o del cliente son inválidos.");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Documento == prenda.Cliente);
                    if (cliente == null)
                    {
                        return BadRequest("El cliente especificado no existe.");
                    }

                    _context.Prendas.Add(prenda);
                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return Ok(new { Mensaje = "Prenda agregada correctamente", PrendaId = prenda.IdPrenda });
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return BadRequest("Error al registrar la prenda.");
                }
            }
        }
    }
}