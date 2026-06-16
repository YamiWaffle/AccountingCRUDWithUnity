using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AccountingApp.Infrastructure.Persistence;

public class AccountingDbContextFactory : IDesignTimeDbContextFactory<AccountingDbContext>
{
    public AccountingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AccountingDbContext>()
            .UseMySql(
                "Server=localhost;Port=3306;Database=accountingdb;User=appuser;Password=apppassword;",
                new MySqlServerVersion(new Version(8, 0, 0)))
            .Options;

        return new AccountingDbContext(options);
    }
}
