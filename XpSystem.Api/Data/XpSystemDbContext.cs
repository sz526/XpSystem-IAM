using Microsoft.EntityFrameworkCore;
using XpSystem;

namespace XpSystem.Api.Data;

public class XpSystemDbContext : DbContext
{
    public XpSystemDbContext(DbContextOptions<XpSystemDbContext> options) : base(options) { }

    public DbSet<PlayerModel> Players => Set<PlayerModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerModel>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd(); // 数据库自增主键
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
        });
    }
}
