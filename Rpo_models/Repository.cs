using Computer_Store.Data;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Computer_Store.Models;

namespace Computer_Store.Rpo_models
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _context;

		public Repository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<Product> GetProductAsync(int productId)
		{
			return await _context.Products
							.Include(p => p.Discounts)
							.Include(p => p.Photos).Include(p => p.Category)
							.FirstOrDefaultAsync(p => p.ProductId == productId);
		}
		public async Task<List<Product>> Sereach(string Name)
		{
			return await _context.Products.Include(p => p.Discounts)
							.Include(p => p.Photos)
							.Include(p => p.Category)
							.Where(
				s => s.Product_Name.Contains(Name) ||
				s.Type.Contains(Name) ||
				s.Description.Contains(Name)
				).ToListAsync();
		}

		public async Task<List<Category>> GetCatAsync()
		{
			return await _context.Categories
							.Include(p => p.Products)
							.ThenInclude(p => p.Photos)
									.Include(p => p.Products)
							.ThenInclude(p => p.Discounts)
							.ToListAsync();
		}

		public void Detach(T entity)
		{
			_context.Entry(entity).State = EntityState.Detached;
		}
		public async Task<IEnumerable<T>> GetAll()
		{
			return await _context.Set<T>().ToListAsync();
		}

		public async Task<T> GetById(int id)
		{
			return await _context.Set<T>().FindAsync(id);
		}
		public async Task<T> GetByemail(string email)
		{
			return await _context.Set<T>().FindAsync(email);
		}
		public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter)
		{
			return await _context.Set<T>().FirstOrDefaultAsync(filter);
		}
		public async Task<T> filterone(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
		{
			IQueryable<T> query = _context.Set<T>();

			if (include != null)
			{
				query = include(query);
			}

			return await query.FirstOrDefaultAsync(filter);
		}

		public async Task Create(T entity)
		{
			_context.Set<T>().Add(entity);
			await _context.SaveChangesAsync();
		}

		public async Task Update(T entity)
		{
			_context.Entry(entity).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task Delete(T entity)
		{
			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync();
		}
		public async Task<IEnumerable<T>> GetAllWithIncludes(params Expression<Func<T, object>>[] includes)
		{
			IQueryable<T> query = _context.Set<T>();

			foreach (var include in includes)
			{
				query = query.Include(include);
			}

			return await query.ToListAsync();
		}

		public async Task<IEnumerable<T>> Include(params Expression<Func<T, object>>[] includes)
		{
			IQueryable<T> query = _context.Set<T>();

			query = includes.Aggregate(query, (current, include) => current.Include(include));

			return await query.ToListAsync();
		}
		public async Task DeleteWhere(Expression<Func<T, bool>> condition)
		{
			var entitiesToDelete = await _context.Set<T>().Where(condition).ToListAsync();
			_context.Set<T>().RemoveRange(entitiesToDelete);
			await _context.SaveChangesAsync();
		}
		public async Task<List<T>> GetFilteredAsync(Expression<Func<T, bool>> filter,
																																																		params Expression<Func<T, object>>[] includes)
		{
			IQueryable<T> query = _context.Set<T>();

			foreach (var include in includes)
			{
				query = query.Include(include);
			}

			if (filter != null)
			{
				query = query.Where(filter);
			}

			return await query.ToListAsync();
		}


	}
}
