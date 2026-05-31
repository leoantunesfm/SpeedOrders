using FillGaps.SpeedOrders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FillGaps.SpeedOrders.Infrastructure.Data;

public class SpeedOrdersDbContext : DbContext
{
    public SpeedOrdersDbContext(DbContextOptions<SpeedOrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpeedOrdersDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}