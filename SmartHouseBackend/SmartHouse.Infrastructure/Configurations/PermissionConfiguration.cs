using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(permission => permission.Id);

            builder.HasOne(permission => permission.Owner).WithMany().OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(permission => permission.Recipient).WithMany().OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(permission => permission.Device).WithMany().OnDelete(DeleteBehavior.NoAction);

        }
    }
}
