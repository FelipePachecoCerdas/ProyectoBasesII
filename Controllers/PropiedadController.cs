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
            _context.Database.SetCommandTimeout(0);
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


        // GET: api/Pais/totales
        [HttpGet("totales")]
        public async Task<IActionResult> GetTotales()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SqlConnection conn = new SqlConnection("Data Source=192.168.1.10\\SQLMASTER;Initial Catalog=Propiedades;Persist Security Info=True;User ID=propiedades;Password=propiedades");
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT CASE WHEN codigoPais IS NULL THEN -1 ELSE codigoPais END, CASE WHEN annoRegistro IS NULL THEN -1 ELSE annoRegistro END, count(*) FROM Propiedad GROUP BY  GROUPING SETS((codigoPais, annoRegistro), (), (codigoPais), (annoRegistro))", conn);
            cmd.CommandTimeout = 0;
            SqlDataReader dr = cmd.ExecuteReader();
            List<Totales> lista = new List<Totales>();

            while (dr.Read())
            {

                Totales p = new Totales();
                p.codigoPais = dr.GetDecimal(0);
                p.annoRegistro = dr.GetDecimal(1);
                p.cantidad = (decimal)dr.GetInt32(2);
                lista.Add(p);
            }

            conn.Close();
            return Ok(lista);
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

            RawSqlString cmd = "SELECT @maximo = CASE WHEN MAX(numeroPlano) IS NULL THEN -1 ELSE MAX(numeroPlano) FROM propiedad";
            SqlParameter maximo = new SqlParameter("@maximo", System.Data.SqlDbType.Int);
            maximo.Direction = System.Data.ParameterDirection.Output;
            _context.Database.SetCommandTimeout(0);
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
        [HttpGet("cantidad/{idPais}")]
        public async Task<IActionResult> GetCantidad([FromRoute] decimal idPais)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //RawSqlString cmd = "SELECT @cantidad = SUM(rows) FROM SYS.partitions WHERE index_id IN(0,1) AND object_id = OBJECT_ID('Propiedad')";
            RawSqlString cmd = "SELECT @cantidad = count(*) FROM Propiedad WHERE codigoPais = " + idPais;
            SqlParameter cantidad = new SqlParameter("@cantidad", System.Data.SqlDbType.Int);
            cantidad.Direction = System.Data.ParameterDirection.Output;
            _context.Database.SetCommandTimeout(0);
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