using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	[DataContract]
	internal abstract class DeleteEntityCommand<TEntitySet> : KeyedEntityCommand<TEntitySet, VoidResult>, IDeleteEntityCommand<TEntitySet>, IKeyedEntityCommand<TEntitySet, VoidResult>, IEntityCommand<TEntitySet, VoidResult> where TEntitySet : IStorageEntitySetScope<IStoreSession>
	{
	}
}
