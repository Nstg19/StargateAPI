using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace StargateAPI.Business.Data.Repositories;

public class Repository<T>: IRepository<T> where T : class
{
	private readonly StargateContext _stargateContext;
	private readonly DbSet<T> _dbSet;

	public Repository(StargateContext stargateContext)
	{
		_stargateContext = stargateContext;
		_dbSet = _stargateContext.Set<T>();
	}

	public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
	{
		return _dbSet.AsNoTracking().Where(predicate);
	}

	public async Task<T> AddAsync(T entity)
	{
		var result = await _dbSet.AddAsync(entity);
		await _stargateContext.SaveChangesAsync();

		return result.Entity;
	}

	public async Task<T> UpdateAsync(T entity)
	{
		var result = _dbSet.Update(entity);
		await _stargateContext.SaveChangesAsync();

		return result.Entity;
	}
}
