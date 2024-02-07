using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.Repositories
{
    public class PanelRepository : RepositoryBase<Panel>
    {
        public PanelRepository(DataContext dataContext) : base(dataContext)
        {

        }
    }
}
