using API_TRADUCTOR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_TRADUCTOR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiTraductorDB _db;

        public UsersController(ApiTraductorDB db)
        {
            _db = db;
        }

        // GET: api/<UserController>
        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            if (_db.Users == null)
            {
                return NotFound();
            }
            return await _db.Users.ToListAsync();
        }

        // GET api/<UserController>/5
        [HttpGet("GetOne/{id}")]
        public async Task<ActionResult<User>> GetOne (int id)
        {
            if (_db.Users == null)
            {
                return NotFound();
            }
            var user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<User>> PostBuilding(User user)
        {
            if (_db.Users == null)
            {
                return Problem("Entity set 'UttnMaintenanceDb.Buildings'  is null.");
            }
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetOne", new { id = user.Id }, user);
        }

        // PUT api/<UserController>/5
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _db.Users.Entry(user).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExist(id))
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

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_db.Users == null)
            {
                return NotFound();
            }
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExist(int id)
        {
            return (_db.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
