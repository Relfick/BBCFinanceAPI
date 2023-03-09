using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BBCFinanceAPI.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBCFinanceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace BBCFinanceAPI.Controllers;

[ApiController]
[Authorize]
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
        return await _db.Users.ToListAsync();
    }
    
    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(long id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        return user;
    }
    
    // POST: api/User
    [HttpPost]
    [AllowAnonymous]
    public async Task<string> RegisterUser(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var claims = new List<Claim> { new(ClaimTypes.Name, user.Id.ToString()) };
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            signingCredentials: new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(jwt);
        
        // return CreatedAtAction(
        //     nameof(GetUser),
        //     new
        //     {
        //         id = user.Id,
        //         first_name = user.FirstName,
        //         username = user.Username,
        //         workMode = user.WorkMode
        //     },
        //     user);
    }
    
    // PUT: api/User/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(long id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest("Id mismatch!");
        }

        _db.Entry(user).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
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
    
    // PUT: api/User/workmode/5
    [HttpPut("workmode/{id}")]
    public async Task<IActionResult> SetWorkMode(long id, [FromBody] UserWorkMode workMode)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        user.WorkMode = workMode;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
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

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(long id)
    {
        var user = _db.Users.Find(id);
        return user != null;
    }
    
    private long? GetTgUserId()
    {
        var identity = ControllerContext.HttpContext.User.Identity;
        if (identity == null)
            return null;
        var id = long.Parse(identity.Name!);
        return id;
    } 
}