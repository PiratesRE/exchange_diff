using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal abstract class MailEnabledOrgPersonSchema : MailEnabledRecipientSchema
	{
		public static readonly ADPropertyDefinition Extensions = UMMailboxSchema.Extensions;

		public static readonly ADPropertyDefinition HasPicture = ADRecipientSchema.HasPicture;

		public static readonly ADPropertyDefinition HasSpokenName = ADRecipientSchema.HasSpokenName;
	}
}
