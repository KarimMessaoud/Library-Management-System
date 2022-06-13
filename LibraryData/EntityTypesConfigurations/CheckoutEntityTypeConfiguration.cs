using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryData.EntityTypesConfigurations
{
    public class CheckoutEntityTypeConfiguration : IEntityTypeConfiguration<Checkout>
    {
        public void Configure(EntityTypeBuilder<Checkout> builder)
        {
            builder.HasOne(x => x.LibraryAsset).WithOne(x => x.Checkout).HasForeignKey<Checkout>(x => x.LibraryAssetId).IsRequired();
            builder.HasOne(x => x.LibraryCard).WithMany(x => x.Checkouts).IsRequired();
        }
    }
}
