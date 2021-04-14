using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal abstract class TenantScopedPropertyCapabilityEvaluator : CapabilityIdentifierEvaluator
	{
		public TenantScopedPropertyCapabilityEvaluator(Capability capability) : base(capability)
		{
		}

		protected IConfigurationSession GetTenantScopedSystemConfigurationSession(OrganizationId userOrgId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), userOrgId, null, false);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.FullyConsistent, sessionSettings, 104, "GetTenantScopedSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ValidationRules\\CapabilityIdentifier.cs");
		}
	}
}
