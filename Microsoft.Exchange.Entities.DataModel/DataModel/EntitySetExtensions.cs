using System;

namespace Microsoft.Exchange.Entities.DataModel
{
	public static class EntitySetExtensions
	{
		public static TEntity Update<TEntity>(this IEntitySet<TEntity> entitySet, TEntity entity, CommandContext context = null) where TEntity : class, IEntity
		{
			return entitySet.Update(entity.Id, entity, context);
		}
	}
}
