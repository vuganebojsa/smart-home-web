using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model.OutsideSmartDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class VehicleGateConfiguration : IEntityTypeConfiguration<VehicleGate>
    {
        public void Configure(EntityTypeBuilder<VehicleGate> builder)
        {
            builder.Property(vg => vg.ValidLicensePlates)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                ).Metadata.SetValueComparer(new StringListValueComparer(deep: true));

        }
    }
}
