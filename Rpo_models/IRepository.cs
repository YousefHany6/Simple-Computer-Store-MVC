using Computer_Store.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Computer_Store.Rpo_models
{
	public interface IRepository<T> where T : class
	{
		Task<List<Product>> Sereach(string Name);
		
			Task<List<Category>> GetCatAsync();

		Task<Product> GetProductAsync(int productId);
		void Detach(T entity);
		Task<IEnumerable<T>> GetAll();
		Task<T> GetById(int id);
		Task<T> GetByemail(string email);
		Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter);
		Task<T> filterone(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
		Task Create(T entity);
		Task Update(T entity);
		Task Delete(T entity);
		Task<IEnumerable<T>> GetAllWithIncludes(params Expression<Func<T, object>>[] includes);

	Task<IEnumerable<T>> Include(params Expression<Func<T, object>>[] includes);

		Task DeleteWhere(Expression<Func<T, bool>> condition);

		Task<List<T>> GetFilteredAsync(Expression<Func<T, bool>> filter,
																																																			params Expression<Func<T, object>>[] includes);
	}
}
