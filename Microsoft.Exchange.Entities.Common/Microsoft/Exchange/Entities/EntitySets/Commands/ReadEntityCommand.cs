using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	internal abstract class ReadEntityCommand<TEntitySet, TEntity> : KeyedEntityCommand<TEntitySet, TEntity>, IReadEntityCommand<TEntitySet, TEntity>, IKeyedEntityCommand<TEntitySet, TEntity>, IEntityCommand<TEntitySet, TEntity> where TEntitySet : IStorageEntitySetScope<IStoreSession> where TEntity : IEntity
	{
	}
}
