using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BBCFinanceAPI.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBCFinanceAPI.Models;
using BBCFinanceAPI.Models.DTO;
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

    // GET: api/Users
    [HttpGet("~api/users")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _db.Users.ToListAsync();
    }
    
    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<User>> GetUser()
    {
        var tgUserId = GetTgUserId();
        var user = await _db.Users.FindAsync(tgUserId);

        if (user == null)
            return NotFound();

        return user;
    }
    
    // POST: api/User
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<string>> RegisterUser(User user)
    {
        try
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return Conflict();
        }

        var claims = new List<Claim> { new(ClaimTypes.Name, user.Id.ToString()) };
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            signingCredentials: new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(jwt);
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

    // GET: api/User/workmode
    [HttpGet("workmode")]
    public async Task<ActionResult<UserWorkMode>> GetWorkMode()
    {
        var tgUserId = GetTgUserId();
        var user = await _db.Users.FindAsync(tgUserId);
        if (user == null)
            return NotFound();

        return user.WorkMode;
    }
    
    // PUT: api/User/workmode
    [HttpPut("workmode")]
    public async Task<IActionResult> SetWorkMode([FromBody] UserWorkMode workMode)
    {
        var tgUserId = GetTgUserId();
        var user = await _db.Users.FindAsync(tgUserId);
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

        return Ok();
    }
    
    // GET: api/User/ExpenseCategories
    [HttpGet("ExpenseCategories")]
    public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetExpenseCategories()
    {
        var userId = GetTgUserId();
        return await _db.ExpenseCategories.Where(ec => ec.UserId == userId).ToListAsync();
    }
    
    // GET: api/User/ExpenseCategory/food
    [HttpGet("ExpenseCategory/{name}")]
    public async Task<ActionResult<ExpenseCategory>> GetExpenseCategory(string name)
    {
        var tgUserId = GetTgUserId();
        
        var category = await _db.ExpenseCategories.FirstOrDefaultAsync(ec => 
            ec.UserId == tgUserId && ec.Name == name);
        if (category == null)
            return NotFound();
        
        return category;
    }

    // POST: api/User/ExpenseCategory
    [HttpPost("ExpenseCategory")]
    public async Task<ActionResult<ExpenseCategory>> PostExpenseCategory(ExpenseCategory expenseCategory)
    {
        var tgUserId = GetTgUserId();
        if (tgUserId != expenseCategory.UserId)
            return BadRequest("Id mismatch!");
        
        _db.ExpenseCategories.Add(expenseCategory);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetExpenseCategory),
            new
            {
                id = expenseCategory.Id,  
                userId = expenseCategory.UserId, 
                name = expenseCategory.Name
            },
            expenseCategory);
    }
    
    // PUT: api/User/ExpenseCategory/5
    [HttpPut("ExpenseCategory/{id}")]
    public async Task<IActionResult> PutExpenseCategory(long id, ExpenseCategory newExpenseCategory)
    {
        if (id != newExpenseCategory.Id)
        {
            return BadRequest("Id mismatch!");
        }

        _db.Entry(newExpenseCategory).State = EntityState.Modified;
        
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExpenseCategoryExists(id))
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
    
    // DELETE: api/User/ExpenseCategory/food
    [HttpDelete("ExpenseCategory/{name}")]
    public async Task<IActionResult> DeleteExpenseCategory(string name)
    {
        var tgUserId = GetTgUserId();
        var expenseCategory = await _db.ExpenseCategories.FirstOrDefaultAsync(ec => 
            ec.UserId == tgUserId && ec.Name == name);
        if (expenseCategory == null)
            return NotFound();
        
        _db.ExpenseCategories.Remove(expenseCategory);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/User/Expenses?categoryName=food
    [HttpGet("Expenses")]
    public async Task<ActionResult<List<ExpenseDTO>>> GetExpenses([FromQuery] string? categoryName)
    {
        var tgUserId = GetTgUserId();

        var expenses = _db.Expenses
            .Include(e => e.ExpenseCategory)
            .Where(e => e.UserId == tgUserId);

        if (categoryName != null)
        {
            expenses = expenses.Where(e => e.ExpenseCategory.Name == categoryName);
        }

        var dto = expenses.Select(
            e => new ExpenseDTO
            {
                ExpenseCategoryName = e.ExpenseCategory.Name,
                Cost = e.Cost,
                Date = e.Date,
                Name = e.Name
            })
            .ToList();
        
        return dto;
    }
    
    // POST: api/User/Expense
    [HttpPost("Expense")]
    public async Task<ActionResult<Expense>> PostExpense(Expense expense)
    {
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetExpenseCategory),
            new
            {
                id = expense.Id,
                expenseCategoryId = expense.ExpenseCategoryId,
                cost = expense.Cost,
                userId = expense.UserId,
                name = expense.Name,
                date = expense.Date
            },
            expense);
    }
    
    private bool UserExists(long id)
    {
        var user = _db.Users.Find(id);
        return user != null;
    }
    
    private bool ExpenseCategoryExists(long id)
    {
        var expenseCategory = _db.ExpenseCategories.Find(id);
        return expenseCategory != null;
    }
    
    private long GetTgUserId()
    {
        var identity = ControllerContext.HttpContext.User.Identity!;
        var id = long.Parse(identity.Name!);
        return id;
    } 
}