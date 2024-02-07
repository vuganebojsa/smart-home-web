using SmartHouse.Core.Model;

namespace SmartHouse.Infrastructure.Repositories
{
    public class UserRepository : RepositoryBase<User>
    {
        public UserRepository(DataContext dataContext) : base(dataContext)
        {
        }
    }
}
