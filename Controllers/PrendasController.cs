using Parcial2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                .Include(c => c.Prendas)
                    .ThenInclude(p => p.Fotos)
                .FirstOrDefaultAsync(c => c.Documento == documento);

            if (cliente == null)
                return NotFound("Cliente no encontrado");

            // Construir la respuesta con los datos requeridos
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
                    Imagenes = p.Fotos.Select(f => f.FotoPrendaUrl).ToList()
                }).ToList()
            };

            return Ok(resultado);
        }

        [HttpPost("subir-imagen")]
        public async Task<IActionResult> SubirImagen(int prendaId, IFormFile imagen)
        {
            // Validar si la prenda existe
            var prenda = await _context.Prendas.FindAsync(prendaId);
            if (prenda == null)
                return NotFound("Prenda no encontrada");

            // Validar si se subió una imagen
            if (imagen == null || imagen.Length == 0)
                return BadRequest("Debe proporcionar una imagen válida.");

            // Generar un nombre único para la imagen
            var nombreArchivo = $"{Guid.NewGuid()}_{imagen.FileName}";
            var ruta = Path.Combine("wwwroot/imagenes", nombreArchivo);

            // Guardar la imagen en el servidor
            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            // Registrar la imagen en la base de datos
            var foto = new FotoPrenda
            {
                IdPrenda = prendaId,
                FotoPrendaUrl = $"/imagenes/{nombreArchivo}"
            };

            _context.FotosPrendas.Add(foto);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Imagen guardada", Url = foto.FotoPrendaUrl });
        }

        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarPrenda([FromBody] Prenda prenda)
        {
            if (prenda == null || string.IsNullOrEmpty(prenda.ClienteDocumento))
            {
                return BadRequest("Los datos de la prenda o del cliente son inválidos.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Buscar si el cliente ya existe
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Documento == prenda.ClienteDocumento);
                if (cliente == null)
                {
                    // Si el cliente no existe, se crea uno nuevo
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

                // Asignar el cliente a la prenda
                prenda.Cliente = cliente;

                // Agregar la prenda a la base de datos
                _context.Prendas.Add(prenda);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

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