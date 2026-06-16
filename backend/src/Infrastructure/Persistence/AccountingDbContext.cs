using AccountingApp.Application.Common;
using AccountingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Infrastructure.Persistence;

public class AccountingDbContext : DbContext, IAccountingDbContext
{
    public AccountingDbContext(DbContextOptions<AccountingDbContext> options) : base(options) { }

    public DbSet<AccountEntry> AccountEntries => Set<AccountEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Category).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(500);
        });
    }
}
