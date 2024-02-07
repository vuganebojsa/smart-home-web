using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.Configurations
{
    public class PanelConfiguration : IEntityTypeConfiguration<Panel>
    {
        public void Configure(EntityTypeBuilder<Panel> builder)
        {

            builder.HasKey(panel => panel.Id);

        }
    }
}
