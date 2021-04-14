using System;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.EntitySets
{
	public interface IEntityCommandFactory<TScope, TEntity> where TEntity : IEntity
	{
		ICreateEntityCommand<TScope, TEntity> CreateCreateCommand(TEntity entity, TScope scope);

		IDeleteEntityCommand<TScope> CreateDeleteCommand(string key, TScope scope);

		IFindEntitiesCommand<TScope, TEntity> CreateFindCommand(IEntityQueryOptions queryOptions, TScope scope);

		IReadEntityCommand<TScope, TEntity> CreateReadCommand(string key, TScope scope);

		IUpdateEntityCommand<TScope, TEntity> CreateUpdateCommand(string key, TEntity entity, TScope scope);
	}
}
