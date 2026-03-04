using Microsoft.EntityFrameworkCore;
using TradePOC.Domain.Aggregates;

namespace TradePOC.Infrastructure
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 模拟数据初始化
            modelBuilder.Entity<Card>().HasData(
                new { CardNo = "6222021234567890", Balance = 10000M, IsActive = true },
                new { CardNo = "6222029876543210", Balance = 5000M, IsActive = true }
            );

            // 索引优化（高并发）
            modelBuilder.Entity<Transaction>().HasIndex(t => t.TransactionId).IsUnique();
            modelBuilder.Entity<Transaction>().HasIndex(t => new { t.CardNo, t.Status });
            modelBuilder.Entity<Card>().HasIndex(c => c.CardNo).IsUnique();
        }
    }
}
