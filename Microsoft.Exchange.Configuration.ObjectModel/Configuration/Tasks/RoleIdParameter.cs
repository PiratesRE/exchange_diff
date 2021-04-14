using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RoleIdParameter : ADIdParameter
	{
		public RoleIdParameter()
		{
		}

		public RoleIdParameter(string identity) : base(identity)
		{
		}

		public RoleIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RoleIdParameter(ExchangeRole role) : base(role.Id)
		{
		}

		public RoleIdParameter(ExchangeRoleAssignmentPresentation assignment) : base(assignment.Role)
		{
		}

		public RoleIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		public static RoleIdParameter Parse(string identity)
		{
			return new RoleIdParameter(identity);
		}
	}
}
