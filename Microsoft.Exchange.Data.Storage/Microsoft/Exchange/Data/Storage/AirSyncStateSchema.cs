using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AirSyncStateSchema : StoreObjectSchema
	{
		public new static AirSyncStateSchema Instance
		{
			get
			{
				if (AirSyncStateSchema.instance == null)
				{
					AirSyncStateSchema.instance = new AirSyncStateSchema();
				}
				return AirSyncStateSchema.instance;
			}
		}

		public static readonly StorePropertyDefinition PropertyGroupChangeMask = InternalSchema.PropertyGroupChangeMask;

		public static readonly StorePropertyDefinition PropertyGroupMappingId = InternalSchema.PropertyGroupMappingId;

		public static readonly StorePropertyDefinition ClientCategoryList = InternalSchema.ClientCategoryList;

		public static readonly StorePropertyDefinition LastSeenClientIds = InternalSchema.LastSeenClientIds;

		public static readonly StorePropertyDefinition LastSyncAttemptTime = InternalSchema.LastSyncAttemptTime;

		public static readonly StorePropertyDefinition LastSyncSuccessTime = InternalSchema.LastSyncSuccessTime;

		public static readonly StorePropertyDefinition LastSyncUserAgent = InternalSchema.LastSyncUserAgent;

		public static readonly StorePropertyDefinition MetadataLastSyncTime = InternalSchema.AirSyncLastSyncTime;

		public static readonly StorePropertyDefinition MetadataLocalCommitTimeMax = InternalSchema.AirSyncLocalCommitTimeMax;

		public static readonly StorePropertyDefinition MetadataDeletedCountTotal = InternalSchema.AirSyncDeletedCountTotal;

		public static readonly StorePropertyDefinition MetadataSyncKey = InternalSchema.AirSyncSyncKey;

		public static readonly StorePropertyDefinition MetadataFilter = InternalSchema.AirSyncFilter;

		public static readonly StorePropertyDefinition MetadataMaxItems = InternalSchema.AirSyncMaxItems;

		public static readonly StorePropertyDefinition MetadataConversationMode = InternalSchema.AirSyncConversationMode;

		public static readonly StorePropertyDefinition MetadataSettingsHash = InternalSchema.AirSyncSettingsHash;

		public static readonly StorePropertyDefinition LastPingHeartbeatInterval = InternalSchema.LastPingHeartbeatInterval;

		public static readonly StorePropertyDefinition DeviceBlockedUntil = InternalSchema.DeviceBlockedUntil;

		public static readonly StorePropertyDefinition DeviceBlockedAt = InternalSchema.DeviceBlockedAt;

		public static readonly StorePropertyDefinition DeviceBlockedReason = InternalSchema.DeviceBlockedReason;

		private static AirSyncStateSchema instance = null;
	}
}
