using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryData.EntityTypesConfigurations
{
    public class LibraryBranchEntityTypeConfiguration : IEntityTypeConfiguration<LibraryBranch>
    {
        public void Configure(EntityTypeBuilder<LibraryBranch> builder)
        {
            builder.HasMany(x => x.Patrons)
                .WithOne(x => x.HomeLibraryBranch);

            builder.HasMany(x => x.LibraryAssets)
                .WithOne(x => x.Location)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Address)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Telephone).HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength(false)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(600);

            builder.Property(x => x.OpenDate)
                .HasColumnType("date");

            builder.Property(x => x.ImageUrl).HasMaxLength(500);
        }
    }
}
