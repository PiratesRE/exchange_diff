using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Entities.DataModel
{
	public interface IEntitySet<TEntity> where TEntity : IEntity
	{
		IQueryable<TEntity> AsQueryable(CommandContext context = null);

		TEntity Create(TEntity entity, CommandContext context = null);

		void Delete(string key, CommandContext context = null);

		int EstimateTotalCount(IEntityQueryOptions queryOptions, CommandContext context = null);

		IEnumerable<TEntity> Find(IEntityQueryOptions queryOptions = null, CommandContext context = null);

		TEntity Read(string key, CommandContext context = null);

		TEntity Update(string key, TEntity entity, CommandContext context = null);
	}
}
