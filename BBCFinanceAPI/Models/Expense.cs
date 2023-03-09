using System.ComponentModel.DataAnnotations;

namespace BBCFinanceAPI.Models;

public class Expense
{
    public int Id { get; set; }
    
    [Required]
    public long UserId { get; set; }
    
    [Required]
    public int ExpenseCategoryId { get; set; }
    
    [MaxLength(5, ErrorMessage = "Название траты должно быть не больше 5 символов")]
    public string Name { get; set; }
    
    [Range(1, 100, ErrorMessage = "Стоимость должна быть не более 100")]
    [Required]
    public int Cost { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public Expense(long userId, string name, int cost, int expenseCategoryId, DateTime date)
    {
        UserId = userId;
        Name = name;
        Cost = cost;
        ExpenseCategoryId = expenseCategoryId;
        Date = date;
    }
}