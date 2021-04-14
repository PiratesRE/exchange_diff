using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IRMConfigurationValidationResultSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Results = new SimpleProviderPropertyDefinition("Results", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
