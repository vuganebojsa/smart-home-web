using Microsoft.EntityFrameworkCore;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using System.Linq.Expressions;

namespace SmartHouse.Infrastructure.Repositories
{
    public class SmartPropertyRepository : RepositoryBase<SmartProperty>, ISmartPropertyRepository
    {
        public SmartPropertyRepository(DataContext dataContext) : base(dataContext)
        {


        }

        public async Task<PagedList<SmartProperty>> FindAllWithUser(Page page)
        {
            return PagedList<SmartProperty>.ToPagedList(await _dataContext.SmartProperties.Where(property => property.IsAccepted == Activation.Accepted && property.User != null).Include(prop => prop.User).ToListAsync(),
           page.PageNumber,
           page.PageSize);

        }

        public async Task<PagedList<SmartProperty>> FindPendingWithUser(Page page)
        {
            return PagedList<SmartProperty>.ToPagedList(await _dataContext.SmartProperties.Where(property => property.IsAccepted == Activation.Pending && property.User != null).Include(prop => prop.User).ToListAsync(),
           page.PageNumber,
           page.PageSize);

        }


        public async Task<ICollection<SmartProperty>> FindByConditionWithUser(Expression<Func<SmartProperty, bool>> expression)
        {
            return await _dataContext.SmartProperties.Where(expression).Include(sp => sp.User).ToListAsync();
        }

        public async Task<SmartProperty> FindByConditionWithUserSingle(Expression<Func<SmartProperty, bool>> expression)
        {
            return await _dataContext.SmartProperties.Where(expression).Include(sp => sp.User).FirstOrDefaultAsync();
        }

        public async Task<ICollection<SmartProperty>> FindTotal(int n)
        {
            return await _dataContext.SmartProperties.Take(n).ToListAsync();
        }

        public async Task<ICollection<Guid>> GetSmartPropertyIds()
        {
            return await _dataContext.SmartProperties.Select(sp => sp.Id).ToListAsync();
        }
    }
}
