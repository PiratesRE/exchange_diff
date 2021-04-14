using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ImportContactListResultSchema : SimpleProviderObjectSchema
	{
		public static readonly ProviderPropertyDefinition ContactsImported = new SimpleProviderPropertyDefinition("ContactsImported", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
