using Parcial2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Parcial2.Controllers
{
    [HttpGet("cliente/{documento}")]
    public class PrendasController : ApiController
    {
        public async Task<IActionResult> ObtenerPrendasPorCliente(string documento)
        {
            // Buscar el cliente con sus prendas e imágenes asociadas
            var cliente = await _context.Clientes
                .Include(c => c.Prendas.Select(p => p.Fotos))
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
                    Imagenes = p.Fotos.Select(f => f.FotoPrendaUrl)
                })
            };

            return Ok(resultado);
        }

        // POST api/prendas/agregar
        [HttpPost]
        [Route("api/prendas/agregar")]
        public async Task<IHttpActionResult> AgregarPrenda([FromBody] Prenda prenda)
        {
            if (prenda == null || string.IsNullOrEmpty(prenda.ClienteDocumento))
                return BadRequest("Los datos de la prenda o del cliente son inválidos.");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Documento == prenda.ClienteDocumento);
                    if (cliente == null)
                    {
                        cliente = new Cliente
                        {
                            Documento = prenda.ClienteDocumento,
                            Nombre = prenda.Cliente.Nombre,
                            Email = prenda.Cliente.Email,
                            Celular = prenda.Cliente.Celular
                        };
                        _context.Clientes.Add(cliente);
                        await _context.SaveChangesAsync();
                    }

                    prenda.Cliente = cliente;
                    _context.Prendas.Add(prenda);
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                return Ok(new { Mensaje = "Prenda agregada correctamente", PrendaId = prenda.IdPrenda });
            }
            catch
            {
                await transaction.RollbackAsync();
                return BadRequest("Error al registrar la prenda.");
            }
        }
     


    }
}