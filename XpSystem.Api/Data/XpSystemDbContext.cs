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
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);

            // 阶段 3 新增的两行，必须在 lambda 里面
            entity.Property(p => p.UserId).IsRequired().HasMaxLength(64);
            entity.HasIndex(p => p.UserId);
        });
    }
}
