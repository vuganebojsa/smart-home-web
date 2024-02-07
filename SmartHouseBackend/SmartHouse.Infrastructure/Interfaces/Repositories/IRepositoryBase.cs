using SmartHouse.Core.Model;
using System.Linq.Expressions;

namespace SmartHouse.Infrastructure.Interfaces.Repositories
{
    public interface IRepositoryBase<T>
    {
        Task<ICollection<T>> FindAll();
        Task<PagedList<T>> FindAll(Page page);
        Task<T?> FindById(Guid id);
        Task<ICollection<T>> FindByCondition(Expression<Func<T, bool>> expression);
        Task<PagedList<T>> FindByCondition(Expression<Func<T, bool>> expression, Page page);
        Task<T> FindSingleByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Delete(T entity);
        Task<ICollection<T>> GetNItems(int totalItems);
        Task SaveChanges();
    }
}
