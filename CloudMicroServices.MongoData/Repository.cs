using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BaseApiArchitecture.Interfaces;
using BaseApiArchitecture.Domain;
using MongoDB.Driver;

namespace CloudMicroServices.MongoData
{
	public class Repository<T> : IRepository<T> where T : BaseEntity
    {
		private IMongoDatabase Context { get; set; }
		protected IMongoCollection<T> DbSet { get; set; }

		public Repository(IUnitOfWork UnitOfWork)
		{
			this.Context = (UnitOfWork as UnitOfWork).Context;
			DbSet = Context.GetCollection<T>("listingsAndReviews");
		}

		public virtual async Task Delete(params T[] Entities)
		{
			await DbSet.DeleteManyAsync(x => Entities.Contains(x));
		}

		public virtual async Task<T> GetById(T Entity)
		{
			return await (await DbSet.FindAsync(x => x == Entity)).FirstOrDefaultAsync();
		}

		public virtual async Task Insert(params T[] Entities)
		{
			await DbSet.InsertManyAsync(Entities);
		}

		public virtual async Task Update(params T[] Entities)
		{
			foreach (var Entity in Entities)
				await DbSet.ReplaceOneAsync(x => x == Entity, Entity);
		}

		public virtual async Task<IEnumerable<T>> GetWithFilter(IFilter<T> BaseFilter)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<T>> GetWithFilter(Expression<Func<T, bool>> Predicate = null, bool ReadOnly = true, int? Page = 1, int? Quantity = 10, params Expression<Func<T, object>>[] Includes)
		{
			IQueryable<T> Set = DbSet.AsQueryable();
			Set.Where(Predicate);

			if (Page.HasValue && Quantity.HasValue)
				Set = Set.Skip((Page.Value - 1) * Quantity.Value).Take(Quantity.Value);

			return Set.ToList();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
