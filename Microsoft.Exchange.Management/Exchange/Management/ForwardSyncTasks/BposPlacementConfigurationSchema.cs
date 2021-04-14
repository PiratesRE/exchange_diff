using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	internal class BposPlacementConfigurationSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Configuration = new SimpleProviderPropertyDefinition("Configuration", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
