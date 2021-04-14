using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TestDataCenterDKMAccessResultSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition AclStateIsGood = new SimpleProviderPropertyDefinition("AclStateIsGood", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AclStateDetails = new SimpleProviderPropertyDefinition("AclStateDetails", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
