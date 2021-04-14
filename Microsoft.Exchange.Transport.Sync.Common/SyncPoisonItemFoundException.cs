using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SyncPoisonItemFoundException : LocalizedException
	{
		public SyncPoisonItemFoundException(string syncPoisonItem, Guid subscriptionId) : base(Strings.SyncPoisonItemFoundException(syncPoisonItem, subscriptionId))
		{
			this.syncPoisonItem = syncPoisonItem;
			this.subscriptionId = subscriptionId;
		}

		public SyncPoisonItemFoundException(string syncPoisonItem, Guid subscriptionId, Exception innerException) : base(Strings.SyncPoisonItemFoundException(syncPoisonItem, subscriptionId), innerException)
		{
			this.syncPoisonItem = syncPoisonItem;
			this.subscriptionId = subscriptionId;
		}

		protected SyncPoisonItemFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.syncPoisonItem = (string)info.GetValue("syncPoisonItem", typeof(string));
			this.subscriptionId = (Guid)info.GetValue("subscriptionId", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("syncPoisonItem", this.syncPoisonItem);
			info.AddValue("subscriptionId", this.subscriptionId);
		}

		public string SyncPoisonItem
		{
			get
			{
				return this.syncPoisonItem;
			}
		}

		public Guid SubscriptionId
		{
			get
			{
				return this.subscriptionId;
			}
		}

		private readonly string syncPoisonItem;

		private readonly Guid subscriptionId;
	}
}
