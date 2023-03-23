using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BBCFinanceAPI.Models;

public class Expense
{
    public long Id { get; set; }
    
    [Required]
    public long UserId { get; set; }
    
    [Required]
    public long ExpenseCategoryId { get; set; }
    [JsonIgnore]
    public ExpenseCategory? ExpenseCategory { get; set; }
    
    [Required]
    [MaxLength(20, ErrorMessage = "Название траты должно быть не больше 20 символов")]
    public string Name { get; set; }
    
    [Range(1, 10000000, ErrorMessage = "Стоимость должна быть не более 1 млн")]
    [Required]
    public int Cost { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public Expense(long userId, string name, int cost, long expenseCategoryId, DateTime date)
    {
        UserId = userId;
        Name = name;
        Cost = cost;
        ExpenseCategoryId = expenseCategoryId;
        Date = date;
    }
}