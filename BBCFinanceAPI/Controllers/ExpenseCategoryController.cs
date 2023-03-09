using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBCFinanceAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace BBCFinanceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ExpenseCategoryController : ControllerBase
{
    private readonly ApplicationContext _db;
    // private readonly long _tgUserId;

    public ExpenseCategoryController(ApplicationContext db)
    {
        _db = db;
    }
    
    // GET: api/ExpenseCategory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetExpenseCategories()
    {
        if (_db.ExpenseCategories == null)
            return NotFound();

        var tgUserId = GetTgUserId();
        if (tgUserId == null)
            return Unauthorized();
        
        return await _db.ExpenseCategories.Where(ec => ec.UserId == tgUserId).ToListAsync();
    }

    // GET: api/ExpenseCategory/5/
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseCategory>> GetExpenseCategory(int id)
    {
        if (_db.ExpenseCategories == null)
            return NotFound();

        var category = await _db.ExpenseCategories.FindAsync(id);
        if (category == null)
            return NotFound();
        
        return category;
    }
    
    // POST: api/ExpenseCategory
    [HttpPost]
    public async Task<ActionResult<ExpenseCategory>> PostExpenseCategory(ExpenseCategory expenseCategory)
    {
        if (_db.ExpenseCategories == null)
        {
            return Problem("Entity set 'ApplicationContext.UserExpenseCategories'  is null.");
        }
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
    
    // PUT: api/ExpenseCategory/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutExpenseCategory(int id, ExpenseCategory newExpenseCategory)
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
    
    // DELETE: api/UserExpenseCategory/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserExpenseCategory(int id)
    {
        if (_db.ExpenseCategories == null)
            return NotFound();

        var expenseCategory = await _db.ExpenseCategories.FindAsync(id);
        if (expenseCategory == null)
            return NotFound();
        
        _db.ExpenseCategories.Remove(expenseCategory);
        await _db.SaveChangesAsync();

        return NoContent();
    }
    
    private bool ExpenseCategoryExists(int id)
    {
        var expenseCategory = _db.ExpenseCategories.Find(id);
        return expenseCategory != null;
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