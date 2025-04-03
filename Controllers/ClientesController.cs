using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcial2.Models;
using System.Linq;
using System.Threading.Tasks;
using VentaRopaAPI.Data;
using VentaRopaAPI.Models;

namespace Parcial2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly TiendaRopaDbContext _context;

        public ClientesController(TiendaRopaDbContext context)
        {
            _context = context;
        }

        // OBTENER TODOS LOS CLIENTES
        [HttpGet]
        public async Task<IActionResult> ObtenerClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return Ok(clientes);
        }

        // OBTENER CLIENTE POR DOCUMENTO
        [HttpGet("{documento}")]
        public async Task<IActionResult> ObtenerCliente(string documento)
        {
            var cliente = await _context.Clientes.FindAsync(documento);
            if (cliente == null)
                return NotFound("Cliente no encontrado.");
            return Ok(cliente);
        }

        // CREAR CLIENTE
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] Cliente cliente)
        {
            if (cliente == null)
                return BadRequest("Datos inválidos.");

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ObtenerCliente), new { documento = cliente.Documento }, cliente);
        }

        // ACTUALIZAR CLIENTE
        [HttpPut("{documento}")]
        public async Task<IActionResult> ActualizarCliente(string documento, [FromBody] Cliente clienteActualizado)
        {
            if (documento != clienteActualizado.Documento)
                return BadRequest("Documento no coincide.");

            _context.Entry(clienteActualizado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ELIMINAR CLIENTE
        [HttpDelete("{documento}")]
        public async Task<IActionResult> EliminarCliente(string documento)
        {
            var cliente = await _context.Clientes.FindAsync(documento);
            if (cliente == null)
                return NotFound("Cliente no encontrado.");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
