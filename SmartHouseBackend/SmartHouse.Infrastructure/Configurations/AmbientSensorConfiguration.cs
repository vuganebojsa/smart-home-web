using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model.SmartHomeDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class AmbientSensorConfiguration : IEntityTypeConfiguration<AmbientSensor>
    {
        public void Configure(EntityTypeBuilder<AmbientSensor> builder)
        {

        }
    }
}
