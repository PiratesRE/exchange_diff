using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UncompressedSyncStateSizeExceededException : SyncStateSizeExceededException
	{
		public UncompressedSyncStateSizeExceededException(string syncStateId, Guid subscriptionId, ByteQuantifiedSize currentUncompressedSyncStateSize, ByteQuantifiedSize loadedSyncStateSizeLimit) : base(Strings.UncompressedSyncStateSizeExceededException(syncStateId, subscriptionId, currentUncompressedSyncStateSize, loadedSyncStateSizeLimit))
		{
			this.syncStateId = syncStateId;
			this.subscriptionId = subscriptionId;
			this.currentUncompressedSyncStateSize = currentUncompressedSyncStateSize;
			this.loadedSyncStateSizeLimit = loadedSyncStateSizeLimit;
		}

		public UncompressedSyncStateSizeExceededException(string syncStateId, Guid subscriptionId, ByteQuantifiedSize currentUncompressedSyncStateSize, ByteQuantifiedSize loadedSyncStateSizeLimit, Exception innerException) : base(Strings.UncompressedSyncStateSizeExceededException(syncStateId, subscriptionId, currentUncompressedSyncStateSize, loadedSyncStateSizeLimit), innerException)
		{
			this.syncStateId = syncStateId;
			this.subscriptionId = subscriptionId;
			this.currentUncompressedSyncStateSize = currentUncompressedSyncStateSize;
			this.loadedSyncStateSizeLimit = loadedSyncStateSizeLimit;
		}

		protected UncompressedSyncStateSizeExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.syncStateId = (string)info.GetValue("syncStateId", typeof(string));
			this.subscriptionId = (Guid)info.GetValue("subscriptionId", typeof(Guid));
			this.currentUncompressedSyncStateSize = (ByteQuantifiedSize)info.GetValue("currentUncompressedSyncStateSize", typeof(ByteQuantifiedSize));
			this.loadedSyncStateSizeLimit = (ByteQuantifiedSize)info.GetValue("loadedSyncStateSizeLimit", typeof(ByteQuantifiedSize));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("syncStateId", this.syncStateId);
			info.AddValue("subscriptionId", this.subscriptionId);
			info.AddValue("currentUncompressedSyncStateSize", this.currentUncompressedSyncStateSize);
			info.AddValue("loadedSyncStateSizeLimit", this.loadedSyncStateSizeLimit);
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

		public ByteQuantifiedSize CurrentUncompressedSyncStateSize
		{
			get
			{
				return this.currentUncompressedSyncStateSize;
			}
		}

		public ByteQuantifiedSize LoadedSyncStateSizeLimit
		{
			get
			{
				return this.loadedSyncStateSizeLimit;
			}
		}

		private readonly string syncStateId;

		private readonly Guid subscriptionId;

		private readonly ByteQuantifiedSize currentUncompressedSyncStateSize;

		private readonly ByteQuantifiedSize loadedSyncStateSizeLimit;
	}
}
