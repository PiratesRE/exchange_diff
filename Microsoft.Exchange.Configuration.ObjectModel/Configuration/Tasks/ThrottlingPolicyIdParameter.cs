using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ThrottlingPolicyIdParameter : ADIdParameter, IIdentityParameter
	{
		public ThrottlingPolicyIdParameter()
		{
		}

		public ThrottlingPolicyIdParameter(string identity) : base(identity)
		{
		}

		public ThrottlingPolicyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ThrottlingPolicyIdParameter(ThrottlingPolicy policy) : base(policy.Id)
		{
		}

		public ThrottlingPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		public static ThrottlingPolicyIdParameter Parse(string identity)
		{
			return new ThrottlingPolicyIdParameter(identity);
		}
	}
}
