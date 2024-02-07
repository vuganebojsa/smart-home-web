using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model.ElectromagneticDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class ElectricVehicleChargerConfiguration : IEntityTypeConfiguration<ElectricVehicleCharger>
    {
        public void Configure(EntityTypeBuilder<ElectricVehicleCharger> builder)
        {

        }
    }
}
