using FillGaps.SpeedOrders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FillGaps.SpeedOrders.Infrastructure.Data.Mappings;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.TotalAmount)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(e => e.Status)
               .HasConversion<int>()
               .IsRequired();
    }
}