using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class MailboxAuditLogSearchSchema : AuditLogSearchBaseSchema
	{
		public static readonly ProviderPropertyDefinition MailboxIds = new SimpleProviderPropertyDefinition("MailboxIds", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ShowDetails = new SimpleProviderPropertyDefinition("ShowDetails", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LogonTypeStrings = new SimpleProviderPropertyDefinition("LogonTypes", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Operations = new SimpleProviderPropertyDefinition("Operations", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
