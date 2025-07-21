using Microsoft.EntityFrameworkCore;
using ContosoPizza.Models;

namespace ContosoPizza.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // 告訴 EF Core：「我有一張資料表叫做 Pizzas，這張表的每一列（row）對應到一個 Pizza 類別的物件。」
    // Pizza 是一個 model class（你的程式中的資料結構）
    // DbSet<Pizza> 是一組可以操作的資料列（CRUD）
    // Pizzas 是你在程式中操作的集合名稱（通常是資料表名稱的複數Table name）

    public DbSet<Pizza> Pizzas { get; set; }
}
