using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ManagementScopeIdParameter : ADIdParameter
	{
		public ManagementScopeIdParameter()
		{
		}

		public ManagementScopeIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ManagementScopeIdParameter(ManagementScope managementScope) : base(managementScope.Id)
		{
		}

		public ManagementScopeIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected ManagementScopeIdParameter(string identity) : base(identity)
		{
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		public static ManagementScopeIdParameter Parse(string identity)
		{
			return new ManagementScopeIdParameter(identity);
		}

		internal static ADObjectId GetRootContainerId(IConfigurationSession scSession)
		{
			ADObjectId orgContainerId = scSession.GetOrgContainerId();
			return orgContainerId.GetDescendantId(ManagementScope.RdnScopesContainerToOrganization);
		}
	}
}
