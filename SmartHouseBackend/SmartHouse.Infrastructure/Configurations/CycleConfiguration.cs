using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.Configurations
{
    public class CycleConfiguration : IEntityTypeConfiguration<Cycle>
    {
        public void Configure(EntityTypeBuilder<Cycle> builder)
        {

            builder.HasKey(cycle => cycle.Id);

        }
    }
}
