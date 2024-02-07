using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model.SmartHomeDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class AirConditioningConfiguration : IEntityTypeConfiguration<AirConditioner>
    {
        public void Configure(EntityTypeBuilder<AirConditioner> builder)
        {
            builder.Property(ac => ac.Modes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => Enum.Parse<Mode>(s)).ToList()
            ).Metadata.SetValueComparer(new ModeListValueComparer());

        }
    }
}
