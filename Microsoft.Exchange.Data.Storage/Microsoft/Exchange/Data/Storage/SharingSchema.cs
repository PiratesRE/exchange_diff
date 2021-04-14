using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharingSchema : Schema
	{
		public new static SharingSchema Instance
		{
			get
			{
				if (SharingSchema.instance == null)
				{
					SharingSchema.instance = new SharingSchema();
				}
				return SharingSchema.instance;
			}
		}

		public static readonly StorePropertyDefinition ExternalSharingSharerIdentity = InternalSchema.ExternalSharingSharerIdentity;

		public static readonly StorePropertyDefinition ExternalSharingSharerName = InternalSchema.ExternalSharingSharerName;

		public static readonly GuidNamePropertyDefinition ExternalSharingRemoteFolderId = InternalSchema.ExternalSharingRemoteFolderId;

		public static readonly GuidNamePropertyDefinition ExternalSharingRemoteFolderName = InternalSchema.ExternalSharingRemoteFolderName;

		public static readonly GuidNamePropertyDefinition ExternalSharingLevelOfDetails = InternalSchema.ExternalSharingLevelOfDetails;

		public static readonly GuidNamePropertyDefinition ExternalSharingIsPrimary = InternalSchema.ExternalSharingIsPrimary;

		public static readonly GuidNamePropertyDefinition ExternalSharingSharerIdentityFederationUri = InternalSchema.ExternalSharingSharerIdentityFederationUri;

		public static readonly GuidNamePropertyDefinition ExternalSharingUrl = InternalSchema.ExternalSharingUrl;

		public static readonly GuidNamePropertyDefinition ExternalSharingLocalFolderId = InternalSchema.ExternalSharingLocalFolderId;

		public static readonly GuidNamePropertyDefinition ExternalSharingDataType = InternalSchema.ExternalSharingDataType;

		public static readonly GuidNamePropertyDefinition ExternalSharingSharingKey = InternalSchema.ExternalSharingSharingKey;

		public static readonly GuidNamePropertyDefinition ExternalSharingSubscriberIdentity = InternalSchema.ExternalSharingSubscriberIdentity;

		public static readonly GuidNamePropertyDefinition ExternalSharingMasterId = InternalSchema.ExternalSharingMasterId;

		public static readonly GuidNamePropertyDefinition ExternalSharingSyncState = InternalSchema.ExternalSharingSyncState;

		private static SharingSchema instance = null;
	}
}
