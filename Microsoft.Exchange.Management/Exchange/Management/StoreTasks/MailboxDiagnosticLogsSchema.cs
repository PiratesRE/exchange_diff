using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxDiagnosticLogsSchema : SimpleProviderObjectSchema
	{
		public static readonly SimplePropertyDefinition MailboxLog = new SimplePropertyDefinition("MailboxLog", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition LogName = new SimplePropertyDefinition("LogName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
