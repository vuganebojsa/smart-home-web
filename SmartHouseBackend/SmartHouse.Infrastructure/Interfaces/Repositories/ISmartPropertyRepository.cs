using SmartHouse.Core.Model;
using System.Linq.Expressions;

namespace SmartHouse.Infrastructure.Interfaces.Repositories
{
    public interface ISmartPropertyRepository : IRepositoryBase<SmartProperty>
    {
        public Task<ICollection<SmartProperty>> FindByConditionWithUser(Expression<Func<SmartProperty, bool>> expression);
        public Task<SmartProperty> FindByConditionWithUserSingle(Expression<Func<SmartProperty, bool>> expression);

        public Task<ICollection<Guid>> GetSmartPropertyIds();
        public Task<PagedList<SmartProperty>> FindAllWithUser(Page page);
        public Task<ICollection<SmartProperty>> FindTotal(int n);

        public Task<PagedList<SmartProperty>> FindPendingWithUser(Page page);

    }
}
