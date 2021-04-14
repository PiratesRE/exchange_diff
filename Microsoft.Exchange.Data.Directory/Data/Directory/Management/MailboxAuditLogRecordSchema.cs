using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MailboxAuditLogRecordSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition MailboxGuid = new SimpleProviderPropertyDefinition("MailboxGuid", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MailboxResolvedOwnerName = new SimpleProviderPropertyDefinition("MailboxResolvedOwnerName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LastAccessed = new SimpleProviderPropertyDefinition("LastAccessed", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
