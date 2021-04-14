using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	public abstract class FindEntitiesCommand<TContext, TEntity> : EntityCommand<TContext, IEnumerable<TEntity>>, IFindEntitiesCommand<TContext, TEntity>, IEntityCommand<TContext, IEnumerable<TEntity>> where TEntity : IEntity
	{
		public IEntityQueryOptions QueryOptions
		{
			get
			{
				return this.queryOptions;
			}
			set
			{
				this.queryOptions = value;
				this.OnQueryOptionsChanged();
			}
		}

		protected virtual void OnQueryOptionsChanged()
		{
		}

		private IEntityQueryOptions queryOptions;
	}
}
