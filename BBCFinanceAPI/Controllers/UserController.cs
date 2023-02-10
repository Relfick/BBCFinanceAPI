using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBCFinanceAPI.Models;

namespace BBCFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationContext _db;

    public UserController(ApplicationContext db)
    {
        _db = db;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        if (_db.Users == null)
        {
            return NotFound();
        }
        return await _db.Users.ToListAsync();
    }
    
    // GET: api/User/5
    [HttpGet("{tgUserId}")]
    public async Task<ActionResult<User>> GetUser(long tgUserId)
    {
        if (_db.Users == null)
            return NotFound();

        var user = await _db.Users.FindAsync(tgUserId);

        if (user == null)
            return NotFound();

        return user;
    }

    // PUT: api/User/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{tgUserId}")]
    public async Task<IActionResult> PutUser(long tgUserId, User user)
    {
        if (tgUserId != user.Id)
        {
            return BadRequest();
        }

        _db.Entry(user).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(tgUserId))
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
    
    [HttpPut("workmode/{tgUserId}")]
    public async Task<IActionResult> PutWorkMode(long tgUserId, [FromBody] UserWorkMode workMode)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == tgUserId);
        if (user == null)
            return NotFound();

        user.WorkMode = workMode;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(tgUserId))
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
    
    // POST: api/User
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        if (_db.Users == null)
        {
            return Problem("Entity set 'ApplicationContext.Users'  is null.");
        }
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // return CreatedAtAction("GetUser", new { id = user.Id }, user);
        return NoContent();
    }

    // DELETE: api/User/5
    [HttpDelete("{tgUserId}")]
    public async Task<IActionResult> DeleteUser(long tgUserId)
    {
        if (_db.Users == null)
        {
            return NotFound();
        }
        var user = await _db.Users.FindAsync(tgUserId);
        if (user == null)
        {
            return NotFound();
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(long tgUserId)
    {
        return (_db.Users?.Any(e => e.Id == tgUserId)).GetValueOrDefault();
    }
}