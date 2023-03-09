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
public class ExpenseController : ControllerBase
{
    private readonly ApplicationContext _db;

    public ExpenseController(ApplicationContext db)
    {
        _db = db;
    }

    // GET: api/Expense
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
    {
        return await _db.Expenses.ToListAsync();
    }

    // GET: api/Expense/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> GetExpense(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense == null)
            return NotFound();
        
        return expense;
    }

    // POST: api/Expense
    [HttpPost]
    public async Task<ActionResult<Expense>> PostExpense(Expense expense)
    {
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetExpense),
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
    
    // PUT: api/Expense/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutExpense(int id, Expense expense)
    {
        if (id != expense.Id)
        {
            return BadRequest("Id mismatch!");
        }

        _db.Entry(expense).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExpenseExists(id))
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
    
    // DELETE: api/Expense/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense == null)
            return NotFound();

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private bool ExpenseExists(int id)
    {
        var expense = _db.Expenses.Find(id);
        return expense != null;
    }
}