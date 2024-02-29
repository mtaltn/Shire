using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shire.WebMvc.Entities.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).UseIdentityColumn();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(128);

        builder.Property(c => c.Price).IsRequired().HasColumnType("decimal(18,2)");

        builder.ToTable("Products");
    }
}
