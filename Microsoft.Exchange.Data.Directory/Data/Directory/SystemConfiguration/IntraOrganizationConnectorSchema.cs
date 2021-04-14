using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class IntraOrganizationConnectorSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition TargetAddressDomains = OrganizationRelationshipSchema.DomainNames;

		public static readonly ADPropertyDefinition DiscoveryEndpoint = OrganizationRelationshipSchema.TargetAutodiscoverEpr;

		public static readonly ADPropertyDefinition Enabled = OrganizationRelationshipSchema.Enabled;
	}
}
