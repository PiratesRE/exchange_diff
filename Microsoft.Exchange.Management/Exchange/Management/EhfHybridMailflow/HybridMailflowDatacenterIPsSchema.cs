using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.EhfHybridMailflow
{
	internal class HybridMailflowDatacenterIPsSchema : SimpleProviderObjectSchema
	{
		internal static SimpleProviderPropertyDefinition DatacenterIPs = new SimpleProviderPropertyDefinition("DatacenterIPs", ExchangeObjectVersion.Exchange2010, typeof(IPRange), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
