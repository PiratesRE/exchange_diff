using System;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	public interface IUpdateEntityCommand<TScope, TEntity> : IKeyedEntityCommand<TScope, TEntity>, IEntityCommand<TScope, TEntity> where TEntity : IEntity
	{
		TEntity Entity { get; set; }
	}
}
