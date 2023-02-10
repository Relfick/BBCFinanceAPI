using System.ComponentModel.DataAnnotations;

namespace BBCFinanceAPI.Models;

public class Expense
{
    public int Id { get; set; }
    public long UserId { get; set; }
    
    // [MaxLength(5, ErrorMessageResourceType = typeof(JSType.Error), ErrorMessageResourceName = "ExpenseNameMaxLength")]
    public string Name { get; set; }
    
    // [MaxLength(40)]
    public string ExpenseCategory { get; set; }
    
    // [Range(1, 100, ErrorMessageResourceType = typeof(JSType.Error), ErrorMessageResourceName = "ExpenseCostRange")]
    public int Cost { get; set; }
    
    public DateTime Date { get; set; }
    
    public Expense(long userId, string name, int cost, string expenseCategory, DateTime date)
    {
        UserId = userId;
        Name = name;
        Cost = cost;
        ExpenseCategory = expenseCategory;
        Date = date;
    }
}