using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace XpSystem.Api.Data;

public class XpSystemDbContextFactory : IDesignTimeDbContextFactory<XpSystemDbContext>
{
    public XpSystemDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<XpSystemDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=xpdb;Username=postgres;Password=mysecretpassword");

        return new XpSystemDbContext(options.Options);
    }
}
