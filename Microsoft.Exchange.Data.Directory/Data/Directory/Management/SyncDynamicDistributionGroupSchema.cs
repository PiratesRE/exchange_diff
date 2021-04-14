using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class SyncDynamicDistributionGroupSchema : DynamicDistributionGroupSchema
	{
		public static readonly ADPropertyDefinition BlockedSendersHash = ADRecipientSchema.BlockedSendersHash;

		public static readonly ADPropertyDefinition RecipientDisplayType = ADRecipientSchema.RecipientDisplayType;

		public static readonly ADPropertyDefinition SafeRecipientsHash = ADRecipientSchema.SafeRecipientsHash;

		public static readonly ADPropertyDefinition SafeSendersHash = ADRecipientSchema.SafeSendersHash;

		public static readonly ADPropertyDefinition EndOfList = SyncMailboxSchema.EndOfList;

		public static readonly ADPropertyDefinition Cookie = SyncMailboxSchema.Cookie;

		public static readonly ADPropertyDefinition DirSyncId = ADRecipientSchema.DirSyncId;
	}
}
