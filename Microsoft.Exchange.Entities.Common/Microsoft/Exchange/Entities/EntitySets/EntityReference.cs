using System;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets
{
	public class EntityReference<TEntitySet, TEntity> : IEntityReference<TEntity> where TEntitySet : IEntitySet<TEntity> where TEntity : class, IEntity
	{
		public EntityReference(TEntitySet entitySet, string key)
		{
			this.EntitySet = entitySet;
			this.EntityKey = key;
		}

		public string EntityKey { get; private set; }

		public TEntitySet EntitySet { get; private set; }

		string IEntityReference<!1>.GetKey()
		{
			return this.EntityKey;
		}

		TEntity IEntityReference<!1>.Read(CommandContext context)
		{
			TEntitySet entitySet = this.EntitySet;
			return entitySet.Read(this.EntityKey, context);
		}
	}
}
