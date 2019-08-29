using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoBasesII.Models;

namespace ProyectoBasesII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropiedadController : ControllerBase
    {
        private readonly PropiedadesContext _context;

        public PropiedadController(PropiedadesContext context)
        {
            _context = context;
        }

        // GET: api/Propiedad
        [HttpGet]
        public IEnumerable<Propiedad> GetPropiedad()
        {
            return _context.Propiedad;
        }



        // GET: api/Propiedad/masPropiedades/5/5
        [HttpGet("masPropiedades/{idPais}/{idPropiedad}")]
        public async Task<IActionResult> GetMasPropiedades([FromRoute] decimal idPais, [FromRoute] decimal idPropiedad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = _context.Propiedad.FromSql("SELECT * FROM propiedad WHERE codigoPais = " + idPais + " ORDER BY numeroPlano OFFSET " + idPropiedad + " ROWS FETCH NEXT 10 ROWS ONLY").ToList();

            return Ok(resultado);
        }

        // GET: api/Propiedad/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropiedad([FromRoute] decimal id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var propiedad = await _context.Propiedad.FindAsync(id);

            if (propiedad == null)
            {
                return NotFound();
            }

            return Ok(propiedad);
        }

        // PUT: api/Propiedad/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPropiedad([FromRoute] decimal id, [FromBody] Propiedad propiedad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != propiedad.NumeroPlano)
            {
                return BadRequest();
            }

            _context.Entry(propiedad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropiedadExists(id))
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

        // POST: api/Propiedad
        [HttpPost]
        public async Task<IActionResult> PostPropiedad([FromBody] Propiedad propiedad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RawSqlString cmd = "SELECT @maximo = MAX(numeroPlano) FROM propiedad";
            SqlParameter maximo = new SqlParameter("@maximo", System.Data.SqlDbType.Int);
            maximo.Direction = System.Data.ParameterDirection.Output;
            _context.Database.ExecuteSqlCommand(cmd, maximo);

            propiedad.NumeroPlano = (decimal)(int)maximo.Value;
            propiedad.NumeroPlano++;
            _context.Propiedad.Add(propiedad);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PropiedadExists(propiedad.NumeroPlano))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPropiedad", new { id = propiedad.NumeroPlano }, propiedad);
        }

        // GET: api/Propiedad/cantidad
        [HttpGet("cantidad")]
        public async Task<IActionResult> GetCantidad()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RawSqlString cmd = "SELECT @cantidad = SUM(rows) FROM SYS.partitions WHERE index_id IN(0,1) AND object_id = OBJECT_ID('Propiedad')";
            SqlParameter cantidad = new SqlParameter("@cantidad", System.Data.SqlDbType.Int);
            cantidad.Direction = System.Data.ParameterDirection.Output;
            _context.Database.ExecuteSqlCommand(cmd, cantidad);

            return Ok(cantidad.Value);
        }

        

        // DELETE: api/Propiedad/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePropiedad([FromRoute] decimal id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var propiedad = await _context.Propiedad.FindAsync(id);
            if (propiedad == null)
            {
                return NotFound();
            }

            _context.Propiedad.Remove(propiedad);
            await _context.SaveChangesAsync();

            return Ok(propiedad);
        }

        private bool PropiedadExists(decimal id)
        {
            return _context.Propiedad.Any(e => e.NumeroPlano == id);
        }
    }
}