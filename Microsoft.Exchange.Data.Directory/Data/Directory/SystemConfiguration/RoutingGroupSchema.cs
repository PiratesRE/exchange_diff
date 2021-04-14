using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class RoutingGroupSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition RoutingMasterDN = new ADPropertyDefinition("RoutingMasterDN", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchRoutingMasterDN", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RoutingGroupMembersDN = new ADPropertyDefinition("RoutingGroupMembersDN", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchRoutingGroupMembersBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
