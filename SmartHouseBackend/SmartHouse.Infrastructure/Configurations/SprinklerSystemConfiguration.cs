using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model.OutsideSmartDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class SprinklerSystemConfiguration : IEntityTypeConfiguration<SprinklerSystem>
    {
        public void Configure(EntityTypeBuilder<SprinklerSystem> builder)
        {
            builder.Property(ss => ss.ActiveDays)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                ).Metadata.SetValueComparer(new StringListValueComparer(deep: true));

        }
    }
}
