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
    public class PrendasController : ApiController
    {
        private readonly AppDbContext _context = new AppDbContext();

        // GET api/prendas
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/prendas/cliente/{documento}
        [HttpGet]
        [Route("api/prendas/cliente/{documento}")]
        public async Task<IHttpActionResult> ObtenerPrendasPorCliente(string documento)
        {
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
                    transaction.Rollback();
                    return BadRequest("Error al registrar la prenda.");
                }
            }
        }

        // POST api/prendas/subir-imagen
        [HttpPost]
        [Route("api/prendas/subir-imagen")]
        public async Task<IHttpActionResult> SubirImagen(int prendaId)
        {
            var httpRequest = HttpContext.Current.Request;
            var postedFile = httpRequest.Files["imagen"];

            if (postedFile == null || postedFile.ContentLength == 0)
                return BadRequest("Debe subir una imagen válida.");

            var prenda = await _context.Prendas.FirstOrDefaultAsync(p => p.IdPrenda == prendaId);
            if (prenda == null)
                return NotFound();

            var nombreArchivo = $"{Guid.NewGuid()}_{Path.GetFileName(postedFile.FileName)}";
            var ruta = Path.Combine(HttpContext.Current.Server.MapPath("~/imagenes"), nombreArchivo);
            postedFile.SaveAs(ruta);

            var foto = new FotoPrenda
            {
                IdPrenda = prendaId,
                FotoPrendaUrl = $"/imagenes/{nombreArchivo}"
            };

            _context.FotosPrendas.Add(foto);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Imagen guardada", Url = foto.FotoPrendaUrl });
        }

        // DELETE api/prendas/eliminar-imagen/{idFoto}
        [HttpDelete]
        [Route("api/prendas/eliminar-imagen/{idFoto}")]
        public async Task<IHttpActionResult> EliminarImagen(int idFoto)
        {
            var foto = await _context.FotosPrendas.FindAsync(idFoto);
            if (foto == null)
                return NotFound();

            var ruta = HttpContext.Current.Server.MapPath("~" + foto.FotoPrendaUrl);
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }

            _context.FotosPrendas.Remove(foto);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Imagen eliminada correctamente" });
        }
    }
}