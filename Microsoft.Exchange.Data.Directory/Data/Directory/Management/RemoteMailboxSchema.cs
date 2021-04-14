using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class RemoteMailboxSchema : MailUserSchema
	{
		public static readonly ADPropertyDefinition RemoteRoutingAddress = ADRecipientSchema.ExternalEmailAddress;

		public static readonly ADPropertyDefinition OnPremisesOrganizationalUnit = ADRecipientSchema.OrganizationalUnit;

		public static readonly ADPropertyDefinition RemoteRecipientType = ADUserSchema.RemoteRecipientType;

		public static readonly ADPropertyDefinition ArchiveState = ADUserSchema.ArchiveState;
	}
}
