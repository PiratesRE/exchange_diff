using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class ClientAccessRulesEvaluationResultSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Name = new SimpleProviderPropertyDefinition("Name", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Action = new SimpleProviderPropertyDefinition("Action", ExchangeObjectVersion.Exchange2012, typeof(ClientAccessRulesAction), PropertyDefinitionFlags.None, ClientAccessRulesAction.AllowAccess, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
