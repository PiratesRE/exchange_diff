using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class SyncDistributionGroupSchema : DistributionGroupSchema
	{
		public static readonly ADPropertyDefinition BlockedSendersHash = ADRecipientSchema.BlockedSendersHash;

		public static readonly ADPropertyDefinition Notes = ADRecipientSchema.Notes;

		public static readonly ADPropertyDefinition RecipientDisplayType = ADRecipientSchema.RecipientDisplayType;

		public static readonly ADPropertyDefinition SafeRecipientsHash = ADRecipientSchema.SafeRecipientsHash;

		public static readonly ADPropertyDefinition SafeSendersHash = ADRecipientSchema.SafeSendersHash;

		public static readonly ADPropertyDefinition EndOfList = SyncMailboxSchema.EndOfList;

		public static readonly ADPropertyDefinition Cookie = SyncMailboxSchema.Cookie;

		public static readonly ADPropertyDefinition DirSyncId = ADRecipientSchema.DirSyncId;

		public new static readonly ADPropertyDefinition Members = ADGroupSchema.Members;

		public static readonly ADPropertyDefinition SeniorityIndex = ADRecipientSchema.HABSeniorityIndex;

		public static readonly ADPropertyDefinition PhoneticDisplayName = ADRecipientSchema.PhoneticDisplayName;

		public static readonly ADPropertyDefinition IsHierarchicalGroup = ADGroupSchema.IsOrganizationalGroup;

		public static readonly ADPropertyDefinition RawManagedBy = ADGroupSchema.RawManagedBy;

		public static readonly ADPropertyDefinition CoManagedBy = ADGroupSchema.CoManagedBy;

		public static readonly ADPropertyDefinition OnPremisesObjectId = ADRecipientSchema.OnPremisesObjectId;

		public static readonly ADPropertyDefinition IsDirSynced = ADRecipientSchema.IsDirSynced;

		public static readonly ADPropertyDefinition DirSyncAuthorityMetadata = ADRecipientSchema.DirSyncAuthorityMetadata;

		public static readonly ADPropertyDefinition ExcludedFromBackSync = ADRecipientSchema.ExcludedFromBackSync;
	}
}
