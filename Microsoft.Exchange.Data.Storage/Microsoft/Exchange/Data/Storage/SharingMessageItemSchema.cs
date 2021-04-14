using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharingMessageItemSchema : MessageItemSchema
	{
		public new static SharingMessageItemSchema Instance
		{
			get
			{
				if (SharingMessageItemSchema.instance == null)
				{
					SharingMessageItemSchema.instance = new SharingMessageItemSchema();
				}
				return SharingMessageItemSchema.instance;
			}
		}

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingProviderGuid = InternalSchema.ProviderGuidBinary;

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingProviderName = InternalSchema.SharingProviderName;

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingProviderUrl = InternalSchema.SharingProviderUrl;

		[Autoload]
		public static readonly StorePropertyDefinition SharingInitiatorName = InternalSchema.SharingInitiatorName;

		[Autoload]
		internal static readonly StorePropertyDefinition SharingInitiatorSmtp = InternalSchema.SharingInitiatorSmtp;

		[Autoload]
		internal static readonly StorePropertyDefinition SharingInitiatorEntryId = InternalSchema.SharingInitiatorEntryId;

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingRemoteType = InternalSchema.SharingRemoteType;

		[Autoload]
		public new static readonly StorePropertyDefinition SharingRemoteName = InternalSchema.SharingRemoteName;

		[Autoload]
		internal static readonly StorePropertyDefinition SharingRemoteUid = InternalSchema.SharingRemoteUid;

		[Autoload]
		internal static readonly StorePropertyDefinition SharingRemoteStoreUid = InternalSchema.SharingRemoteStoreUid;

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingLocalType = InternalSchema.SharingLocalType;

		[Autoload]
		public new static readonly StorePropertyDefinition SharingLocalName = InternalSchema.SharingLocalName;

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingLocalUid = InternalSchema.SharingLocalUid;

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingLocalStoreUid = InternalSchema.SharingLocalStoreUid;

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingCapabilities = InternalSchema.SharingCapabilities;

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingFlavor = InternalSchema.SharingFlavor;

		[Autoload]
		internal new static readonly StorePropertyDefinition SharingDetail = InternalSchema.SharingDetail;

		[Autoload]
		internal static readonly StorePropertyDefinition SharingPermissions = InternalSchema.SharingPermissions;

		[Autoload]
		internal static readonly StorePropertyDefinition SharingResponseType = InternalSchema.SharingResponseType;

		[Autoload]
		internal static readonly StorePropertyDefinition SharingResponseTime = InternalSchema.SharingResponseTime;

		[Autoload]
		internal static readonly StorePropertyDefinition SharingOriginalMessageEntryId = InternalSchema.SharingOriginalMessageEntryId;

		[Autoload]
		internal static readonly StorePropertyDefinition SharingLastSubscribeTime = InternalSchema.SharingLastSubscribeTime;

		[Autoload]
		public new static readonly StorePropertyDefinition SharingRemotePath = InternalSchema.SharingRemotePath;

		[Autoload]
		public static readonly StorePropertyDefinition SharingBrowseUrl = InternalSchema.SharingBrowseUrl;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingBrowseUrl = InternalSchema.XSharingBrowseUrl;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingCapabilities = InternalSchema.XSharingCapabilities;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingFlavor = InternalSchema.XSharingFlavor;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingInstanceGuid = InternalSchema.XSharingInstanceGuid;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingLocalType = InternalSchema.XSharingLocalType;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingProviderGuid = InternalSchema.XSharingProviderGuid;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingProviderName = InternalSchema.XSharingProviderName;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingProviderUrl = InternalSchema.XSharingProviderUrl;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingRemoteName = InternalSchema.XSharingRemoteName;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingRemotePath = InternalSchema.XSharingRemotePath;

		[Autoload]
		internal static readonly StorePropertyDefinition XSharingRemoteType = InternalSchema.XSharingRemoteType;

		private static SharingMessageItemSchema instance = null;
	}
}
