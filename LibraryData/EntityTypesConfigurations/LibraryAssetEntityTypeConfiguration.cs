using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryData.EntityTypesConfigurations
{
    public class LibraryAssetEntityTypeConfiguration : IEntityTypeConfiguration<LibraryAsset>
    {
        public void Configure(EntityTypeBuilder<LibraryAsset> builder)
        {
            builder.Ignore(x => x.EncryptedId);

            builder.HasOne(x => x.Location).WithMany(x => x.LibraryAssets).IsRequired();

            builder.HasOne(x => x.Status);

            builder.Property(x => x.Title).HasMaxLength(250).IsRequired();

            builder.Property(x => x.Cost).HasColumnType("decimal(10,2)");

            builder.Property(x => x.ImageUrl).HasMaxLength(500);

            builder.Property(x => x.NumberOfCopies).IsRequired();

            builder.Property("Discriminator").HasMaxLength(200);
        }
    }
}
