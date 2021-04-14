using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets
{
	internal abstract class StorageEntitySet<TEntitySet, TEntity, TSession> : StorageEntitySet<TEntitySet, TEntity, IEntityCommandFactory<TEntitySet, TEntity>, TSession> where TEntitySet : class, IEntitySet<TEntity> where TEntity : class, IEntity where TSession : class, IStoreSession
	{
		protected StorageEntitySet(IStorageEntitySetScope<TSession> parentScope, string relativeName, IEntityCommandFactory<TEntitySet, TEntity> commandFactory) : base(parentScope, relativeName, commandFactory)
		{
		}
	}
}
