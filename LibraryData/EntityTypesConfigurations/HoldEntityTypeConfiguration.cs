using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryData.EntityTypesConfigurations
{
    public class HoldEntityTypeConfiguration : IEntityTypeConfiguration<Hold>
    {
        public void Configure(EntityTypeBuilder<Hold> builder)
        {
            builder.HasOne(x => x.LibraryAsset)
                .WithOne()
                .HasForeignKey<Hold>(x => x.LibraryAssetId)
                .IsRequired();

            builder.HasOne(x => x.LibraryCard)
                .WithMany()
                .IsRequired();
        }
    }
}
