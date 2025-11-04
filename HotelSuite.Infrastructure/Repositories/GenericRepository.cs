using HotelSuite.Domain.Interfaces;
using HotelSuite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelSuite.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly HotelDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(HotelDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet
         .AsNoTracking()
       .ToListAsync();
    }

    public IQueryable<T> GetAllQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
}

    public void Delete(T entity)
    {
_dbSet.Remove(entity);
 }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
