using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model.SmartHomeDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class WashingMachineConfiguration : IEntityTypeConfiguration<WashingMachine>
    {
        public void Configure(EntityTypeBuilder<WashingMachine> builder)
        {
            builder.HasMany(wm => wm.SupportedCycles).WithMany();
            builder.HasOne(wm => wm.CurrentCycle);
        }
    }
}
