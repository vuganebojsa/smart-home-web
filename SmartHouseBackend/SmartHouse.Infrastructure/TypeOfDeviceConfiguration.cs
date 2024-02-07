using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure
{
    public class TypeOfDeviceConfiguration : IEntityTypeConfiguration<TypeOfDevice>
    {
        public void Configure(EntityTypeBuilder<TypeOfDevice> builder)
        {
            builder.HasKey(device => device.Id);
        }
    }
}
