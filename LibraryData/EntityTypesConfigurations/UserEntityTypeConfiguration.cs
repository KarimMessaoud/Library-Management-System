using LibraryData.Models;
using LibraryData.Models.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LibraryData.EntityTypesConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.HasOne(x => x.LibraryCard)
                    .WithOne(x => x.Patron)
                    .HasForeignKey<LibraryCard>(x => x.PatronId);

                builder.HasOne(x => x.HomeLibraryBranch)
                    .WithMany(x => x.Patrons)
                    .HasForeignKey("HomeLibraryBranchId");

                builder.Property(x => x.FirstName)
                   .HasMaxLength(50)
                   .IsRequired();

                builder.Property(x => x.LastName)
                   .HasMaxLength(50)
                   .IsRequired();

                builder.Property(x => x.Address)
                   .HasMaxLength(100)
                   .IsRequired();

                builder.Property(x => x.Email)
                    .HasMaxLength(256)
                    .IsRequired();

                builder.Property(x => x.DateOfBirth)
                    .HasColumnType("date")
                    .IsRequired();
                

                //The below configuration is based on the following article: https://joshthecoder.com/2020/04/28/max-data-types-in-aspnet-core-identity-schema.html
                //and: https://github.com/dotnet/aspnetcore/issues/5823
                builder.Property(x => x.PhoneNumber).HasMaxLength(50)
                    .IsUnicode(false)
                    .IsFixedLength(false);

                builder.Property(x => x.PasswordHash).HasMaxLength(84)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                builder.Property(x => x.SecurityStamp).HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength(false)
                    .IsRequired(true);

                builder.Property(x => x.ConcurrencyStamp).HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength(true)
                    .IsRequired(true);
            }
    }
}
