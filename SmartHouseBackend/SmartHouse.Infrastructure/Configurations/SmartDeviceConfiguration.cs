using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.Configurations
{
    public class SmartDeviceConfiguration : IEntityTypeConfiguration<SmartDevice>
    {
        public void Configure(EntityTypeBuilder<SmartDevice> builder)
        {
            builder.HasKey(device => device.Id);

        }
    }
}
