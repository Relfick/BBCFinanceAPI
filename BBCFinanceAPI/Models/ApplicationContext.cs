using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace BBCFinanceAPI.Models;

public sealed class ApplicationContext: DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) 
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(UserConfigure);
        modelBuilder.Entity<Expense>(ExpenseConfigure);
        modelBuilder.Entity<ExpenseCategory>(ExpenseCategoryConfigure);
    }
    
    private void UserConfigure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Id).HasColumnName("id");
        builder.Property(u => u.FirstName).HasColumnName("first_name");
        builder.Property(u => u.Username).HasColumnName("username");
        builder.Property(u => u.WorkMode).HasColumnName("workmode");
    }
    
    private void ExpenseConfigure(EntityTypeBuilder<Expense> builder)
    {
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e=> e.Name).HasColumnName("name");
        builder.Property(e=> e.ExpenseCategory).HasColumnName("expense_category");
        builder.Property(e=> e.Cost).HasColumnName("cost");
        builder.Property(e=> e.Date).HasColumnName("date");
    }
    
    private void ExpenseCategoryConfigure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.UserId).HasColumnName("user_id");
        builder.Property(c=> c.Category).HasColumnName("expense_category");
    }
}