using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model.ElectromagneticDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class SolarPanelSystemConfiguration : IEntityTypeConfiguration<SolarPanelSystem>
    {
        public void Configure(EntityTypeBuilder<SolarPanelSystem> builder)
        {
            builder.HasMany(sp => sp.Panels).WithOne(sp => sp.SolarPanelSystem);
        }
    }
}
