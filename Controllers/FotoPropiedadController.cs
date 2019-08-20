using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoBasesII.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ProyectoBasesII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FotoPropiedadController : ControllerBase
    {
        private readonly PropiedadesContext _context;

        public FotoPropiedadController(PropiedadesContext context)
        {
            _context = context;
        }

        // GET: api/FotoPropiedad
        [HttpGet]
        public IEnumerable<FotoPropiedad> GetFotoPropiedad()
        {
            return _context.FotoPropiedad;
        }

        // GET: api/FotoPropiedad/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFotoPropiedad([FromRoute] decimal id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fotoPropiedad = await _context.FotoPropiedad.FindAsync(id);

            if (fotoPropiedad == null)
            {
                return NotFound();
            }

            return Ok(fotoPropiedad);
        }

        // GET: api/FotoPropiedad/archivo/ruta
        [HttpGet("archivo/{ruta}")]
        public async Task<IActionResult> GetArchivo([FromRoute] string ruta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ruta = ruta.Replace('&','\\');

            RawSqlString cmd = "EXEC getArchivo @resultado = @res OUTPUT, @ruta = @rta";
            SqlParameter res = new SqlParameter("@res", System.Data.SqlDbType.VarBinary, -1);
            res.Direction = System.Data.ParameterDirection.Output;
            SqlParameter rta = new SqlParameter("@rta", System.Data.SqlDbType.VarChar, 300);
            rta.Direction = System.Data.ParameterDirection.Input;
            rta.Value = ruta;
            _context.Database.ExecuteSqlCommand(cmd, res, rta);

            if (res.Value == null) {
                return NoContent();
            }
            Byte[] idk = (Byte[]) res.Value;

            //System.Drawing.Image x = (Bitmap)((new System.Drawing.Conv()))
            //System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(idk));
            //image.Save("output.jpg", ImageFormat.Jpeg);
            String formato = ruta.Substring(ruta.Length - 3);
            switch (formato) {
                case "jpg":
                    return File(idk, MediaTypeNames.Image.Jpeg);
                case "pdf":
                    return File(idk, MediaTypeNames.Application.Pdf);
                case "mp4":
                    return File(idk, MediaTypeNames.Application.Octet);
                default:
                    return File(idk, MediaTypeNames.Application.Octet);
            }
        }

        // GET: api/fotoPropiedad/propiedad/5
        [HttpGet("propiedad/{idPropiedad}")]
        public async Task<IActionResult> GetMasPaises([FromRoute] decimal idPropiedad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = _context.FotoPropiedad.FromSql("SELECT * FROM fotoPropiedad WHERE numeroPlanoPropiedad = " + idPropiedad).ToList();

            return Ok(resultado);
        }

        // PUT: api/FotoPropiedad/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFotoPropiedad([FromRoute] decimal id, [FromBody] FotoPropiedad fotoPropiedad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fotoPropiedad.NumeroPlanoPropiedad)
            {
                return BadRequest();
            }

            _context.Entry(fotoPropiedad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FotoPropiedadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FotoPropiedad
        [HttpPost]
        public async Task<IActionResult> PostFotoPropiedad([FromBody] FotoPropiedad fotoPropiedad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FotoPropiedad.Add(fotoPropiedad);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FotoPropiedadExists(fotoPropiedad.NumeroPlanoPropiedad))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFotoPropiedad", new { id = fotoPropiedad.NumeroPlanoPropiedad }, fotoPropiedad);
        }

        // DELETE: api/FotoPropiedad/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFotoPropiedad([FromRoute] decimal id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fotoPropiedad = await _context.FotoPropiedad.FindAsync(id);
            if (fotoPropiedad == null)
            {
                return NotFound();
            }

            _context.FotoPropiedad.Remove(fotoPropiedad);
            await _context.SaveChangesAsync();

            return Ok(fotoPropiedad);
        }

        private bool FotoPropiedadExists(decimal id)
        {
            return _context.FotoPropiedad.Any(e => e.NumeroPlanoPropiedad == id);
        }
    }
}