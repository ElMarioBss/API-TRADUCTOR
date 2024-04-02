using API_TRADUCTOR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_TRADUCTOR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoriesController : ControllerBase
    {
        private readonly ApiTraductorDB _db;

        public HistoriesController(ApiTraductorDB db)
        {
            _db = db;
        }

        // GET: api/<HistoryController>
        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<History>>> Get()
        {
            var userLoggedID = Convert.ToInt32(User.Identity.Name);

            if (_db.Histories == null)
            {
                return NotFound();
            }
            return await _db.Histories.Where(x => x.UserId == userLoggedID).ToListAsync();
        }

        // GET api/<HistoryController>/5
        [HttpGet("GetOne/{id}")]
        public async Task<ActionResult<History>> GetOne(int id)
        {
            if (_db.Histories == null)
            {
                return NotFound();
            }
            var History = await _db.Histories.FindAsync(id);

            if (History == null)
            {
                return NotFound();
            }

            return History;
        }

        // POST api/<HistoryController>
        [HttpPost]
        public async Task<ActionResult<History>> PostBuilding(History History)
        {
            if (_db.Histories == null)
            {
                return Problem("Entity set 'UttnMaintenanceDb.Buildings'  is null.");
            }
            _db.Histories.Add(History);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetOne", new { id = History.Id }, History);
        }

        // PUT api/<HistoryController>/5
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, History History)
        {
            if (id != History.Id)
            {
                return BadRequest();
            }

            _db.Histories.Entry(History).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistoryExist(id))
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

        // DELETE api/<HistoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_db.Histories == null)
            {
                return NotFound();
            }
            var History = await _db.Histories.FindAsync(id);
            if (History == null)
            {
                return NotFound();
            }

            _db.Histories.Remove(History);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool HistoryExist(int id)
        {
            return (_db.Histories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
