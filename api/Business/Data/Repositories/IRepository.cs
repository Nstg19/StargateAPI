using System.Linq.Expressions;

namespace StargateAPI.Business.Data.Repositories;

public interface IRepository<T>
{
	IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
	Task<T> AddAsync(T entity);
	Task<T> UpdateAsync(T entity);
}
