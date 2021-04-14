using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CompressedSyncStateSizeExceededException : SyncStateSizeExceededException
	{
		public CompressedSyncStateSizeExceededException(string syncStateId, Guid subscriptionId, StoragePermanentException e) : base(Strings.CompressedSyncStateSizeExceededException(syncStateId, subscriptionId, e))
		{
			this.syncStateId = syncStateId;
			this.subscriptionId = subscriptionId;
			this.e = e;
		}

		public CompressedSyncStateSizeExceededException(string syncStateId, Guid subscriptionId, StoragePermanentException e, Exception innerException) : base(Strings.CompressedSyncStateSizeExceededException(syncStateId, subscriptionId, e), innerException)
		{
			this.syncStateId = syncStateId;
			this.subscriptionId = subscriptionId;
			this.e = e;
		}

		protected CompressedSyncStateSizeExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.syncStateId = (string)info.GetValue("syncStateId", typeof(string));
			this.subscriptionId = (Guid)info.GetValue("subscriptionId", typeof(Guid));
			this.e = (StoragePermanentException)info.GetValue("e", typeof(StoragePermanentException));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("syncStateId", this.syncStateId);
			info.AddValue("subscriptionId", this.subscriptionId);
			info.AddValue("e", this.e);
		}

		public string SyncStateId
		{
			get
			{
				return this.syncStateId;
			}
		}

		public Guid SubscriptionId
		{
			get
			{
				return this.subscriptionId;
			}
		}

		public StoragePermanentException E
		{
			get
			{
				return this.e;
			}
		}

		private readonly string syncStateId;

		private readonly Guid subscriptionId;

		private readonly StoragePermanentException e;
	}
}
