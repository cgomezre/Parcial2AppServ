using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using Parcial2.Models;
using Parcial2.Data;
using System.Net;

namespace Parcial2.Controllers
{
    [RoutePrefix("api/fotoprendas")]
    public class FotoPrendasController : ApiController
    {
        private readonly AppDbContext _context;

        public FotoPrendasController()
        {
            _context = new AppDbContext();
        }

        // GET: api/fotoprendas
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObtenerImagenes()
        {
            var imagenes = await _context.FotoPrendas.ToListAsync();
            return Ok(imagenes);
        }

        // GET: api/fotoprendas/prenda/{idPrenda}
        [HttpGet]
        [Route("prenda/{idPrenda}")]
        public async Task<IHttpActionResult> ObtenerImagenesDePrenda(int idPrenda)
        {
            var imagenes = await _context.FotoPrendas
                                         .Where(f => f.idPrenda == idPrenda)
                                         .ToListAsync();
            if (!imagenes.Any())
                return NotFound();
            return Ok(imagenes);
        }

        // PUT: api/fotoprendas/{idFoto}
        [HttpPut]
        [Route("{idFoto}")]
        public async Task<IHttpActionResult> ActualizarImagen(int idFoto, [FromBody] FotoPrenda_Models nuevaFoto)
        {
            if (idFoto != nuevaFoto.idFoto)
                return BadRequest();

            _context.Entry(nuevaFoto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/fotoprendas/{idFoto}
        [HttpDelete]
        [Route("{idFoto}")]
        public async Task<IHttpActionResult> EliminarImagen(int idFoto)
        {
            var foto = await _context.FotoPrendas.FindAsync(idFoto);
            if (foto == null)
                return NotFound();

            _context.FotoPrendas.Remove(foto);
            await _context.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
