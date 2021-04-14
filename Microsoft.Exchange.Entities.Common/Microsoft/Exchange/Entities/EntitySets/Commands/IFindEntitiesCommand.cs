using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	public interface IFindEntitiesCommand<TScope, out TEntity> : IEntityCommand<TScope, IEnumerable<TEntity>> where TEntity : IEntity
	{
		IEntityQueryOptions QueryOptions { get; set; }
	}
}
