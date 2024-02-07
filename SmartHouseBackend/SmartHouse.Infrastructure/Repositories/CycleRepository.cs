using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.Repositories
{
    public class CycleRepository : RepositoryBase<Cycle>
    {
        public CycleRepository(DataContext dataContext) : base(dataContext)
        {

        }
    }
}
