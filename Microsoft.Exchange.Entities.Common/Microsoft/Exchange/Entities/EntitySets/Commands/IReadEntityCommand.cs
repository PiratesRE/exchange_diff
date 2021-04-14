using System;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	public interface IReadEntityCommand<TScope, out TEntity> : IKeyedEntityCommand<TScope, TEntity>, IEntityCommand<TScope, TEntity> where TEntity : IEntity
	{
	}
}
