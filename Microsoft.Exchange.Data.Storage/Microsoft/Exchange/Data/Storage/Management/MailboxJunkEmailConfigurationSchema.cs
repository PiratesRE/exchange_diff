using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxJunkEmailConfigurationSchema : XsoMailboxConfigurationObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Identity = XsoMailboxConfigurationObjectSchema.MailboxOwnerId;

		public static readonly SimplePropertyDefinition Enabled = new SimplePropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, true, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition TrustedListsOnly = new SimplePropertyDefinition("TrustedListsOnly", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition ContactsTrusted = new SimplePropertyDefinition("ContactsTrusted", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, true, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition TrustedSendersAndDomains = new SimplePropertyDefinition("TrustedSendersAndDomains", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition BlockedSendersAndDomains = new SimplePropertyDefinition("BlockedSendersAndDomains", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
