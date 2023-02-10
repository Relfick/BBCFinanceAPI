using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBCFinanceAPI.Models;

namespace BBCFinanceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserExpenseCategoryController : ControllerBase
{
    private readonly ApplicationContext _db;

    public UserExpenseCategoryController(ApplicationContext db)
    {
        _db = db;
    }

    // GET: api/UserExpenseCategory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetUserExpenseCategories()
    {
        if (_db.ExpenseCategories == null)
        {
            return NotFound();
        }
        return await _db.ExpenseCategories.ToListAsync();
    }
        
    // GET: api/UserExpenseCategory/5/еда/
    [HttpGet("{userId}/{expenseCategory}")]
    public async Task<ActionResult<ExpenseCategory>> GetUserExpenseCategory(long userId, string expenseCategory)
    {
        // TODO: Проверить
        var userExpenseCategory = await _db.ExpenseCategories.FirstOrDefaultAsync(e => e.UserId == userId && e.Category == expenseCategory);
        if (userExpenseCategory == null)
            return NotFound();

        return userExpenseCategory;
    }

    // GET: api/UserExpenseCategory/5/
    [HttpGet("{userId}")]
    public async Task<ActionResult<List<string>>> GetUserExpenseCategories(long userId)
    {
        if (_db.ExpenseCategories == null)
        {
            return NotFound();
        }

        var categories = await _db.ExpenseCategories.Where(user => user.UserId == userId)
            .Select(c => c.Category)
            .ToListAsync();
        return categories;
    }

    // PUT: api/UserExpenseCategory/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{userId}")]
    public async Task<IActionResult> PutUserExpenseCategory(long userId, Dictionary<string, string> categoriesDictionary)
    {
        var oldCategory = categoriesDictionary["oldCategory"];
        var newCategory = categoriesDictionary["newCategory"];
        
        var oldUserExpenseCategory = await _db.ExpenseCategories.FirstOrDefaultAsync(
            u => u.UserId == userId && u.Category == oldCategory);
        if (oldUserExpenseCategory == null)
            return NotFound();
        
        oldUserExpenseCategory.Category = newCategory;

        var userExpenses = await _db.Expenses
            .Where(ue => ue.UserId == userId && ue.ExpenseCategory == oldCategory)
            .ToListAsync();
        
        foreach (var ue in userExpenses)
        {
            ue.ExpenseCategory = newCategory;
        }
        
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExpenseCategoryExists(userId))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/UserExpenseCategory
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ExpenseCategory>> PostUserExpenseCategory(ExpenseCategory userExpenseCategory)
    {
        if (_db.ExpenseCategories == null)
        {
            return Problem("Entity set 'ApplicationContext.UserExpenseCategories'  is null.");
        }
        _db.ExpenseCategories.Add(userExpenseCategory);
        await _db.SaveChangesAsync();

        // return CreatedAtAction("GetUserExpenseCategory", new { id = userExpenseCategory.Id }, userExpenseCategory);
        return NoContent();
    }

    // DELETE: api/UserExpenseCategory/5/food
    [HttpDelete("{userId}/{category}")]
    public async Task<IActionResult> DeleteUserExpenseCategory(long userId, string category)
    {
        if (_db.ExpenseCategories == null)
        {
            return NotFound();
        }
        
        var userExpenseCategory = await _db.ExpenseCategories.FirstOrDefaultAsync(u => u.UserId == userId && u.Category == category);
        if (userExpenseCategory == null)
            return NotFound();
        
        var userExpensesWithCategory = await _db.Expenses
            .Where(e => e.UserId == userId && e.ExpenseCategory == category)
            .ToListAsync();

        _db.ExpenseCategories.Remove(userExpenseCategory);
        _db.Expenses.RemoveRange(userExpensesWithCategory);
        
        await _db.SaveChangesAsync();

        return NoContent();
    }
    
    private bool UserExpenseCategoryExists(long id)
    {
        return (_db.ExpenseCategories?.Any(e => e.Id == id)).GetValueOrDefault();
    }

}