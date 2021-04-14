using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	[DataContract]
	internal abstract class UpdateEntityCommand<TEntitySet, TEntity> : KeyedEntityCommand<TEntitySet, TEntity>, IUpdateEntityCommand<TEntitySet, TEntity>, IKeyedEntityCommand<TEntitySet, TEntity>, IEntityCommand<TEntitySet, TEntity> where TEntitySet : IStorageEntitySetScope<IStoreSession> where TEntity : IEntity
	{
		protected UpdateEntityCommand()
		{
			base.RegisterOnBeforeExecute(new Action(this.EnforceContextConditions));
		}

		[DataMember]
		public TEntity Entity { get; set; }

		protected override void UpdateCustomLoggingData()
		{
			base.UpdateCustomLoggingData();
			this.SetCustomLoggingData("InputEntity", this.Entity);
		}

		protected abstract void SetETag(string eTag);

		private void EnforceContextConditions()
		{
			TEntity entity = this.Entity;
			entity.Id = base.EntityKey;
			if (this.Context != null && !string.IsNullOrEmpty(this.Context.IfMatchETag))
			{
				this.SetETag(this.Context.IfMatchETag);
			}
		}
	}
}
