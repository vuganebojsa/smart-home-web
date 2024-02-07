using Microsoft.EntityFrameworkCore;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using System.Linq.Expressions;

namespace SmartHouse.Infrastructure.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected DataContext _dataContext { get; set; }
        public RepositoryBase(DataContext dataContext) => _dataContext = dataContext;
        public void Create(T entity) => _dataContext.Set<T>().Add(entity);
        public void Delete(T entity) => _dataContext.Set<T>().Remove(entity);
        public async Task<ICollection<T>> FindAll() => await _dataContext.Set<T>().ToListAsync();
        public async Task<PagedList<T>> FindAll(Page page)
        {
            return PagedList<T>.ToPagedList(await _dataContext.Set<T>().ToListAsync(),
            page.PageNumber,
            page.PageSize);
        }
        public async Task<ICollection<T>> FindByCondition(Expression<Func<T, bool>> expression) => await _dataContext.Set<T>().Where(expression).AsQueryable().ToListAsync();

        public async Task<T> FindSingleByCondition(Expression<Func<T, bool>> expression) => await _dataContext.Set<T>().Where(expression).FirstOrDefaultAsync();
        public async Task<T?> FindById(Guid id) => await _dataContext.Set<T>().FindAsync(id);
        public async Task SaveChanges() => await _dataContext.SaveChangesAsync();

        public async Task<ICollection<T>> GetNItems(int totalItems) => await _dataContext.Set<T>().Take(totalItems).ToListAsync();

        public async Task<PagedList<T>> FindByCondition(Expression<Func<T, bool>> expression, Page page)
        {
            return PagedList<T>.ToPagedList(await _dataContext.Set<T>().Where(expression).ToListAsync(),
           page.PageNumber,
           page.PageSize);
        }
    }
}
