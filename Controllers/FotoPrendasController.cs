using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcial2.Models;
using System.Linq;
using System.Threading.Tasks;
using VentaRopaAPI.Data;
using VentaRopaAPI.Models;

namespace VentaRopaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FotoPrendasController : ControllerBase
    {
        private readonly TiendaRopaDbContext _context;

        public FotoPrendasController(TiendaRopaDbContext context)
        {
            _context = context;
        }

        //OBTENER TODAS LAS IMÁGENES
        [HttpGet]
        public async Task<IActionResult> ObtenerImagenes()
        {
            var imagenes = await _context.FotoPrendas.ToListAsync();
            return Ok(imagenes);
        }

        //OBTENER IMÁGENES DE UNA PRENDA POR ID
        [HttpGet("prenda/{idPrenda}")]
        public async Task<IActionResult> ObtenerImagenesDePrenda(int idPrenda)
        {
            var imagenes = await _context.FotoPrendas
                                         .Where(f => f.IdPrenda == idPrenda)
                                         .ToListAsync();
            if (!imagenes.Any())
                return NotFound("No hay imágenes para esta prenda.");
            return Ok(imagenes);
        }

        //ACTUALIZAR UNA IMAGEN
        [HttpPut("{idFoto}")]
        public async Task<IActionResult> ActualizarImagen(int idFoto, [FromBody] FotoPrenda nuevaFoto)
        {
            if (idFoto != nuevaFoto.IdFoto)
                return BadRequest("El ID de la imagen no coincide.");

            _context.Entry(nuevaFoto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //ELIMINAR UNA IMAGEN
        [HttpDelete("{idFoto}")]
        public async Task<IActionResult> EliminarImagen(int idFoto)
        {
            var foto = await _context.FotoPrendas.FindAsync(idFoto);
            if (foto == null)
                return NotFound("Imagen no encontrada.");

            _context.FotoPrendas.Remove(foto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
