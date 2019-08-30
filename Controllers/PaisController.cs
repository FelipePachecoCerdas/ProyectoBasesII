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
    public class PaisController : ControllerBase
    {
        private readonly PropiedadesContext _context;

        public PaisController(PropiedadesContext context)
        {
            _context = context;
        }

        // GET: api/Pais
        [HttpGet]
        public IEnumerable<Pais> GetPais()
        {
            return _context.Pais;
        }

        // GET: api/Pais/masPaises/5
        [HttpGet("masPaises/{id}")]
        public async Task<IActionResult> GetMasPaises([FromRoute] decimal id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultado = _context.Pais.FromSql("SELECT * FROM pais ORDER BY idPais OFFSET " + id + " ROWS FETCH NEXT 5 ROWS ONLY").ToList();

            return Ok(resultado);
        }

        // GET: api/Pais/cantidad
        [HttpGet("cantidad")]
        public async Task<IActionResult> GetCantidad()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RawSqlString cmd = "SELECT @cantidad = SUM(rows) FROM SYS.partitions WHERE index_id IN(0,1) AND object_id = OBJECT_ID('Pais')";
            
            SqlParameter cantidad = new SqlParameter("@cantidad", System.Data.SqlDbType.Int);
            cantidad.Direction = System.Data.ParameterDirection.Output;
            _context.Database.SetCommandTimeout(0);
            _context.Database.ExecuteSqlCommand(cmd, cantidad);

            return Ok(cantidad.Value);
        }
        
        // GET: api/Pais/propiedadesTipo/5
        [HttpGet("propiedadesTipo/{idPais}")]
        public async Task<IActionResult> GetPropiedadesPorTipo([FromRoute] decimal idPais)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SqlConnection conn = new SqlConnection("Data Source=192.168.1.10\\SQLMASTER;Initial Catalog=Propiedades;Persist Security Info=True;User ID=propiedades;Password=propiedades");
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT pa.idPais, pa.nbrPais, pr.tipoPropiedad, count(*) FROM Pais pa INNER JOIN Propiedad pr ON(pa.idPais = pr.codigoPais) WHERE pa.idPais = " + idPais + " GROUP BY pa.idPais, pa.nbrPais, pr.tipoPropiedad", conn);
            cmd.CommandTimeout = 0;
            SqlDataReader dr = cmd.ExecuteReader();
            List<PropiedadTipo> lista = new List<PropiedadTipo>();

            while(dr.Read()) {
                
                PropiedadTipo p = new PropiedadTipo();
                p.IdPais =  dr.GetDecimal(0);;
                p.NbrPais = dr.GetString(1);
                p.TipoPropiedad =  dr.GetString(2);
                p.Cantidad = (decimal) dr.GetInt32(3);
                lista.Add(p);
            }

            conn.Close();
            return Ok(lista);
        }

        // GET: api/Pais/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPais([FromRoute] decimal id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pais = await _context.Pais.FindAsync(id);

            if (pais == null)
            {
                return NotFound();
            }

            return Ok(pais);
        }

        // PUT: api/Pais/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPais([FromRoute] decimal id, [FromBody] Pais pais)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pais.IdPais)
            {
                return BadRequest();
            }

            _context.Entry(pais).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaisExists(id))
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

        // POST: api/Pais
        [HttpPost]
        public async Task<IActionResult> PostPais([FromBody] Pais pais)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RawSqlString cmd = "SELECT @maximo = MAX(idPais) FROM pais";
            SqlParameter maximo = new SqlParameter("@maximo", System.Data.SqlDbType.Int);
            maximo.Direction = System.Data.ParameterDirection.Output;
            _context.Database.ExecuteSqlCommand(cmd, maximo);

            pais.IdPais = (decimal) (int) maximo.Value;
            pais.IdPais++;
            _context.Pais.Add(pais);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PaisExists(pais.IdPais))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPais", new { id = pais.IdPais }, pais);
        }

        // DELETE: api/Pais/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePais([FromRoute] decimal id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pais = await _context.Pais.FindAsync(id);
            if (pais == null)
            {
                return NotFound();
            }

            _context.Pais.Remove(pais);
            await _context.SaveChangesAsync();

            return Ok(pais);
        }

        private bool PaisExists(decimal id)
        {
            return _context.Pais.Any(e => e.IdPais == id);
        }
    }
}