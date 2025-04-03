using Parcial2.Models;
using Parcial2.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Parcial2.Controllers
{
    [RoutePrefix("api/clientes")]
    public class ClientesController : ApiController
    {
        private readonly AppDbContext _context;

        public ClientesController()
        {
            _context = new AppDbContext();
        }

        // GET: api/clientes
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObtenerClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return Ok(clientes);
        }

        // GET: api/clientes/{documento}
        [HttpGet]
        [Route("{documento}")]
        public async Task<IHttpActionResult> ObtenerCliente(string documento)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Prendas.Select(p => p.FotoPrendas))
                .FirstOrDefaultAsync(c => c.Documento == documento);

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CrearCliente([FromBody] Cliente_Models cliente)
        {
            if (cliente == null)
                return BadRequest("Datos inválidos.");

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Created($"api/clientes/{cliente.Documento}", cliente);
        }

        // PUT: api/clientes/{documento}
        [HttpPut]
        [Route("{documento}")]
        public async Task<IHttpActionResult> ActualizarCliente(string documento, [FromBody] Cliente clienteActualizado)
        {
            if (documento != clienteActualizado.Documento)
                return BadRequest("Documento no coincide.");

            _context.Entry(clienteActualizado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (!_context.Clientes.Any(c => c.Documento == documento))
                    return NotFound();

                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/clientes/{documento}
        [HttpDelete]
        [Route("{documento}")]
        public async Task<IHttpActionResult> EliminarCliente(string documento)
        {
            var cliente = await _context.Clientes.FindAsync(documento);
            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}