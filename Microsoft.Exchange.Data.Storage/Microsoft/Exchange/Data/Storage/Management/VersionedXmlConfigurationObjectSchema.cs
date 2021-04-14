using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class VersionedXmlConfigurationObjectSchema : XsoMailboxConfigurationObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Identity = XsoMailboxConfigurationObjectSchema.MailboxOwnerId;
	}
}
