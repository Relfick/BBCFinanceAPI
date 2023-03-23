using System.ComponentModel.DataAnnotations;

namespace BBCFinanceAPI.Models;

public class ExpenseCategory
{
    public long Id { get; set; }
    
    [Required]
    public long UserId { get; set; }
    
    [Required]
    [MaxLength(15, ErrorMessage = "Длина категории должна быть не более 15 символов")]
    [RegularExpression(@"^[a-zA-Zа-яА-Я\d]+$", ErrorMessage = "Категория должна состоять из одного слова")]
    public string Name { get; set; }

    public ExpenseCategory(long userId, string name)
    {
        UserId = userId;
        Name = name;
    }
}