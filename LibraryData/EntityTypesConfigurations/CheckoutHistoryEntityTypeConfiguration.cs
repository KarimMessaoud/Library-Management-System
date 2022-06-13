using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryData.EntityTypesConfigurations
{
    public class CheckoutHistoryEntityTypeConfiguration : IEntityTypeConfiguration<CheckoutHistory>
    {
        public void Configure(EntityTypeBuilder<CheckoutHistory> builder)
        {
            builder.HasOne(x => x.LibraryAsset)
                .WithMany()
                .IsRequired();

            builder.HasOne(x => x.LibraryCard)
                .WithMany()
                .IsRequired();

            builder.Property(x => x.CheckedOut)
                .IsRequired();
        }
    }
}
