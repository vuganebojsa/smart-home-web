using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model.OutsideSmartDevices;

namespace SmartHouse.Infrastructure.Configurations
{
    public class LampConfiguration : IEntityTypeConfiguration<Lamp>
    {
        public void Configure(EntityTypeBuilder<Lamp> builder)
        {

        }
    }
}
