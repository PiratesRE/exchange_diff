using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class BindingItemSchema : ItemSchema
	{
		public new static BindingItemSchema Instance
		{
			get
			{
				if (BindingItemSchema.instance == null)
				{
					BindingItemSchema.instance = new BindingItemSchema();
				}
				return BindingItemSchema.instance;
			}
		}

		private static BindingItemSchema instance;

		public static readonly StorePropertyDefinition SharingStatus = InternalSchema.SharingStatus;

		public static readonly StorePropertyDefinition SharingProviderGuid = InternalSchema.SharingProviderGuid;

		public static readonly StorePropertyDefinition SharingProviderName = InternalSchema.SharingProviderName;

		public static readonly StorePropertyDefinition SharingProviderUrl = InternalSchema.SharingProviderUrl;

		public static readonly StorePropertyDefinition SharingRemotePath = InternalSchema.SharingRemotePath;

		public static readonly StorePropertyDefinition SharingRemoteName = InternalSchema.SharingRemoteName;

		public static readonly StorePropertyDefinition SharingLocalName = InternalSchema.SharingLocalName;

		public static readonly StorePropertyDefinition SharingLocalUid = InternalSchema.SharingLocalUid;

		public static readonly StorePropertyDefinition SharingLocalType = InternalSchema.SharingLocalType;

		public static readonly StorePropertyDefinition SharingFlavor = InternalSchema.SharingFlavor;

		public static readonly StorePropertyDefinition SharingInstanceGuid = InternalSchema.SharingInstanceGuid;

		public static readonly StorePropertyDefinition SharingRemoteType = InternalSchema.SharingRemoteType;

		public static readonly StorePropertyDefinition SharingLastSync = InternalSchema.SharingLastSync;

		public static readonly StorePropertyDefinition SharingRemoteLastMod = InternalSchema.SharingRemoteLastMod;

		public static readonly StorePropertyDefinition SharingConfigUrl = InternalSchema.SharingConfigUrl;

		public static readonly StorePropertyDefinition SharingDetail = InternalSchema.SharingDetail;

		public static readonly StorePropertyDefinition SharingTimeToLive = InternalSchema.SharingTimeToLive;

		public static readonly StorePropertyDefinition SharingBindingEid = InternalSchema.SharingBindingEid;

		public static readonly StorePropertyDefinition SharingIndexEid = InternalSchema.SharingIndexEid;

		public static readonly StorePropertyDefinition SharingRemoteComment = InternalSchema.SharingRemoteComment;

		public static readonly StorePropertyDefinition SharingLocalStoreUid = InternalSchema.SharingLocalStoreUid;

		public static readonly StorePropertyDefinition SharingRemoteByteSize = InternalSchema.SharingRemoteByteSize;

		public static readonly StorePropertyDefinition SharingRemoteCrc = InternalSchema.SharingRemoteCrc;

		public static readonly StorePropertyDefinition SharingLastAutoSync = InternalSchema.SharingLastAutoSync;

		public static readonly StorePropertyDefinition SharingSavedSession = InternalSchema.SharingSavedSession;

		public static readonly StorePropertyDefinition SharingInitiatorName = InternalSchema.SharingInitiatorName;

		public static readonly StorePropertyDefinition SharingInitiatorSmtp = InternalSchema.SharingInitiatorSmtp;

		public static readonly StorePropertyDefinition SharingRemoteFolderId = InternalSchema.SharingRemoteFolderId;

		public static readonly StorePropertyDefinition SharingRoamLog = InternalSchema.SharingRoamLog;

		public static readonly StorePropertyDefinition SharingLocalFolderEwsId = InternalSchema.SharingLocalFolderEwsId;
	}
}
