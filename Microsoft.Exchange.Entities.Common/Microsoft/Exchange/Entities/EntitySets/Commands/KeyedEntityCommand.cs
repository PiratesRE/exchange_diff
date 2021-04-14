using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	[DataContract]
	internal abstract class KeyedEntityCommand<TEntitySet, TResult> : EntityCommand<TEntitySet, TResult>, IKeyedEntityCommand<TEntitySet, TResult>, IEntityCommand<TEntitySet, TResult> where TEntitySet : IStorageEntitySetScope<IStoreSession>
	{
		[DataMember]
		public string EntityKey { get; set; }

		protected virtual StoreId GetEntityStoreId()
		{
			string changeKey = (this.Context == null) ? null : this.Context.IfMatchETag;
			TEntitySet scope = this.Scope;
			return scope.IdConverter.ToStoreId(this.EntityKey, changeKey);
		}

		protected override string GetCommandTraceDetails()
		{
			return string.Format("({0})", this.EntityKey);
		}

		protected override void UpdateCustomLoggingData()
		{
			base.UpdateCustomLoggingData();
			this.SetCustomLoggingData("InputKey", this.EntityKey);
		}
	}
}
