using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id);
            builder.HasMany(user => user.Properties).WithOne(property => property.User);
            builder.HasIndex(user => user.Email).IsUnique();
            builder.HasIndex(user => user.UserName).IsUnique();
        }
    }
}
