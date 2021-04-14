using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	[DataContract]
	internal abstract class UpdateStorageEntityCommand<TEntitySet, TEntity> : UpdateEntityCommand<TEntitySet, TEntity> where TEntitySet : IStorageEntitySetScope<IStoreSession> where TEntity : IEntity, IVersioned
	{
		protected override void SetETag(string eTag)
		{
			TEntity entity = base.Entity;
			entity.ChangeKey = eTag;
		}
	}
}
