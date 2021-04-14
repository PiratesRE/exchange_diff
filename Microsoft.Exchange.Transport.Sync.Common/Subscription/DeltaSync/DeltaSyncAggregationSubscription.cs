using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class DeltaSyncAggregationSubscription : WindowsLiveServiceAggregationSubscription
	{
		public DeltaSyncAggregationSubscription()
		{
			base.SubscriptionProtocolName = "Delta Sync";
			base.SubscriptionProtocolVersion = 131078;
			base.SubscriptionType = AggregationSubscriptionType.DeltaSyncMail;
		}

		public override string IncomingServerName
		{
			get
			{
				if (string.IsNullOrEmpty(base.IncommingServerUrl))
				{
					return "DeltaSyncMailProxyEndPoint";
				}
				return base.IncommingServerUrl;
			}
		}

		public int MinSyncPollInterval
		{
			get
			{
				return this.minSyncPollInterval;
			}
			set
			{
				this.minSyncPollInterval = value;
			}
		}

		public int MinSettingPollInterval
		{
			get
			{
				return this.minSettingPollInterval;
			}
			set
			{
				this.minSettingPollInterval = value;
			}
		}

		public double SyncMultiplier
		{
			get
			{
				return this.syncMultiplier;
			}
			set
			{
				this.syncMultiplier = value;
			}
		}

		public int MaxObjectInSync
		{
			get
			{
				return this.maxObjectInSync;
			}
			set
			{
				this.maxObjectInSync = value;
			}
		}

		public int MaxNumberOfEmailAdds
		{
			get
			{
				return this.maxNumberOfEmailAdds;
			}
			set
			{
				this.maxNumberOfEmailAdds = value;
			}
		}

		public int MaxNumberOfFolderAdds
		{
			get
			{
				return this.maxNumberOfFolderAdds;
			}
			set
			{
				this.maxNumberOfFolderAdds = value;
			}
		}

		public int MaxAttachments
		{
			get
			{
				return this.maxAttachments;
			}
			set
			{
				this.maxAttachments = value;
			}
		}

		public long MaxMessageSize
		{
			get
			{
				return this.maxMessageSize;
			}
			set
			{
				this.maxMessageSize = value;
			}
		}

		public int MaxRecipients
		{
			get
			{
				return this.maxRecipients;
			}
			set
			{
				this.maxRecipients = value;
			}
		}

		public DeltaSyncAccountStatus AccountStatus
		{
			get
			{
				return this.accountStatus;
			}
			set
			{
				this.accountStatus = value;
			}
		}

		public override int? EnumeratedItemsLimitPerConnection
		{
			get
			{
				return new int?(Math.Max(0, Math.Min(Math.Min(this.maxNumberOfFolderAdds, this.maxObjectInSync), this.maxNumberOfEmailAdds)));
			}
		}

		public override FolderSupport FolderSupport
		{
			get
			{
				return FolderSupport.RootFoldersOnly;
			}
		}

		public override ItemSupport ItemSupport
		{
			get
			{
				return ItemSupport.Email | ItemSupport.Generic;
			}
		}

		public override SyncQuirks SyncQuirks
		{
			get
			{
				return SyncQuirks.EnumerateItemChangeAsDeleteAndAdd | SyncQuirks.DoNotTerminateSlowSyncs;
			}
		}

		public override PimSubscriptionProxy CreateSubscriptionProxy()
		{
			return new HotmailSubscriptionProxy(this);
		}

		protected override void SetPropertiesToMessageObject(MessageItem message)
		{
			base.SetPropertiesToMessageObject(message);
			message[StoreObjectSchema.ItemClass] = "IPM.Aggregation.DeltaSync";
			message[MessageItemSchema.SharingProviderGuid] = DeltaSyncAggregationSubscription.DeltaSyncProviderGuid;
			message[AggregationSubscriptionMessageSchema.SharingMinSyncPollInterval] = this.MinSyncPollInterval;
			message[AggregationSubscriptionMessageSchema.SharingMinSettingPollInterval] = this.MinSettingPollInterval;
			message[AggregationSubscriptionMessageSchema.SharingSyncMultiplier] = this.SyncMultiplier;
			message[AggregationSubscriptionMessageSchema.SharingMaxObjectsInSync] = this.MaxObjectInSync;
			message[AggregationSubscriptionMessageSchema.SharingMaxNumberOfEmails] = this.MaxNumberOfEmailAdds;
			message[AggregationSubscriptionMessageSchema.SharingMaxNumberOfFolders] = this.MaxNumberOfFolderAdds;
			message[AggregationSubscriptionMessageSchema.SharingMaxAttachments] = this.MaxAttachments;
			message[AggregationSubscriptionMessageSchema.SharingMaxMessageSize] = this.MaxMessageSize;
			message[AggregationSubscriptionMessageSchema.SharingMaxRecipients] = this.MaxRecipients;
			message[MessageItemSchema.SharingDetail] = (((int)message[MessageItemSchema.SharingDetail] & -769) | (int)this.AccountStatus);
		}

		protected override void LoadProperties(MessageItem message)
		{
			base.LoadProperties(message);
			base.GetIntProperty(message, AggregationSubscriptionMessageSchema.SharingMinSyncPollInterval, null, null, out this.minSyncPollInterval);
			base.GetIntProperty(message, AggregationSubscriptionMessageSchema.SharingMinSettingPollInterval, null, null, out this.minSettingPollInterval);
			base.GetDoubleProperty(message, AggregationSubscriptionMessageSchema.SharingSyncMultiplier, null, null, out this.syncMultiplier);
			base.GetIntProperty(message, AggregationSubscriptionMessageSchema.SharingMaxObjectsInSync, null, null, out this.maxObjectInSync);
			base.GetIntProperty(message, AggregationSubscriptionMessageSchema.SharingMaxNumberOfEmails, null, null, out this.maxNumberOfEmailAdds);
			base.GetIntProperty(message, AggregationSubscriptionMessageSchema.SharingMaxNumberOfFolders, null, null, out this.maxNumberOfFolderAdds);
			base.GetIntProperty(message, AggregationSubscriptionMessageSchema.SharingMaxAttachments, null, null, out this.maxAttachments);
			base.GetLongProperty(message, AggregationSubscriptionMessageSchema.SharingMaxMessageSize, null, null, out this.maxMessageSize);
			base.GetIntProperty(message, AggregationSubscriptionMessageSchema.SharingMaxRecipients, null, null, out this.maxRecipients);
			base.GetEnumProperty<DeltaSyncAccountStatus>(message, MessageItemSchema.SharingDetail, new int?(768), out this.accountStatus);
		}

		protected override bool ValidateIncomingServerUrl(string incomingServerUrl)
		{
			return Uri.CheckHostName(incomingServerUrl) != UriHostNameType.Unknown;
		}

		protected override void Serialize(AggregationSubscription.SubscriptionSerializer subscriptionSerializer)
		{
			subscriptionSerializer.SerializeDeltaSyncSubscription(this);
		}

		protected override void Deserialize(AggregationSubscription.SubscriptionDeserializer deserializer)
		{
			deserializer.DeserializeDeltaSyncSubscription(this);
		}

		private const string DefaultDeltaSyncIncomingServerName = "DeltaSyncMailProxyEndPoint";

		private const string DeltaSyncProtocolName = "Delta Sync";

		private const int DeltaSyncProtocolVersion = 131078;

		private static readonly Guid DeltaSyncProviderGuid = new Guid("633ebb53-78bf-4eb0-a849-2447b815d5c7");

		private int minSyncPollInterval;

		private int minSettingPollInterval;

		private double syncMultiplier;

		private int maxObjectInSync;

		private int maxNumberOfEmailAdds;

		private int maxNumberOfFolderAdds;

		private int maxAttachments;

		private long maxMessageSize;

		private int maxRecipients;

		private DeltaSyncAccountStatus accountStatus;
	}
}
