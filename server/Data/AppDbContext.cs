using Microsoft.EntityFrameworkCore;
using Chat.Models;

namespace App.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Conversation> Conversations { get; set; }

    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 設定 Conversation 和 Message 的一對多關聯
        modelBuilder.Entity<Conversation>()
            .HasMany(c => c.Messages)                      // Conversation 有很多 Messages
            .WithOne(m => m.Conversation)                  // 每個 Message 對應一個 Conversation
            .HasForeignKey(m => m.ConversationId)          // Message 使用 ConversationId 當外鍵
            .OnDelete(DeleteBehavior.Cascade);             // 若刪除某個 Conversation，相關的 Messages 也會一併刪除（連鎖刪除）

        // 設定 Message 中 Role 欄位的儲存格式
        modelBuilder.Entity<Message>()
            .Property(m => m.Role)
            .HasConversion<string>()                       // 將 enum RoleType 以字串方式存入資料庫（預設是 int）
            .HasMaxLength(50);                             // 設定資料庫中 Role 欄位的最大長度為 50 字元
    }

}

