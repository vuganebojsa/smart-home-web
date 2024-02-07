using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.Configurations
{
    public class SmartPropertyConfiguration : IEntityTypeConfiguration<SmartProperty>
    {
        public void Configure(EntityTypeBuilder<SmartProperty> builder)
        {

            builder.HasKey(property => property.Id);

            builder.HasMany(property => property.Devices).WithOne(device => device.SmartProperty);


        }
    }
}
