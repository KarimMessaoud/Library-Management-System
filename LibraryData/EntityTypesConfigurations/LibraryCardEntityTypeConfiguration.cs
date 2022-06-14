using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryData.EntityTypesConfigurations
{
    public class LibraryCardEntityTypeConfiguration : IEntityTypeConfiguration<LibraryCard>
    {
        public void Configure(EntityTypeBuilder<LibraryCard> builder)
        {
            builder.HasMany(x => x.Checkouts)
                .WithOne(x => x.LibraryCard);

            builder.Property(x => x.Fees)
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.Created)
                .HasColumnType("date")
                .IsRequired();
        }
    }
}
