using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model.ElectromagneticDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class HouseBatteryConfiguration : IEntityTypeConfiguration<HouseBattery>
    {
        public void Configure(EntityTypeBuilder<HouseBattery> builder)
        {

        }
    }
}
