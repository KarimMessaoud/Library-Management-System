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
                .WithMany()
                .IsRequired();
            //.HasForeignKey<Hold>(x => x.LibraryAssetId)

            builder.HasOne(x => x.LibraryCard)
                .WithMany()
                .IsRequired();
        }
    }
}
