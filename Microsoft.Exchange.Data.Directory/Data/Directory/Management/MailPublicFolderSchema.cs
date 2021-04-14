using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MailPublicFolderSchema : MailEnabledRecipientSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADPublicFolderSchema>();
		}

		public static readonly ADPropertyDefinition Contacts = ADPublicFolderSchema.Contacts;

		public static readonly ADPropertyDefinition ContentMailbox = ADRecipientSchema.DefaultPublicFolderMailbox;

		public static readonly ADPropertyDefinition DeliverToMailboxAndForward = ADPublicFolderSchema.DeliverToMailboxAndForward;

		public static readonly ADPropertyDefinition EntryId = ADPublicFolderSchema.EntryId;

		public static readonly ADPropertyDefinition ExternalEmailAddress = ADRecipientSchema.ExternalEmailAddress;

		public static readonly ADPropertyDefinition ForwardingAddress = ADRecipientSchema.ForwardingAddress;

		public static readonly ADPropertyDefinition PhoneticDisplayName = ADRecipientSchema.PhoneticDisplayName;
	}
}
