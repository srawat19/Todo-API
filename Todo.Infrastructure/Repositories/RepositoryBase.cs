using Microsoft.EntityFrameworkCore;
using Todo.Application.Interfaces.Repositories;



namespace Todo.Infrastructure.Repositories
{
    public class RepositoryBase<T, TKey> : IRepositoryBase<T, TKey> where T : class
    {
        protected readonly TodoAppDbContext _todoDbContext;
        protected readonly DbSet<T> _dbSet;
        public RepositoryBase(TodoAppDbContext todoAppDbContext)
        {
            _todoDbContext = todoAppDbContext;
            _dbSet = _todoDbContext.Set<T>();

        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }



        public virtual async Task<T?> GetByIdAsync(TKey Id)
        {
            return await _dbSet.FindAsync(Id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _todoDbContext.SaveChangesAsync();
            return entity;

        }


        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _todoDbContext.SaveChangesAsync();
        }


        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _todoDbContext.SaveChangesAsync();
            return entity;
        }
    }
}
