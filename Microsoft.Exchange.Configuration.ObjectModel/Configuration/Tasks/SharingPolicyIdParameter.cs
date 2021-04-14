using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class SharingPolicyIdParameter : ADIdParameter
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		public SharingPolicyIdParameter()
		{
		}

		public SharingPolicyIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public SharingPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public SharingPolicyIdParameter(string identity) : base(identity)
		{
		}

		public static SharingPolicyIdParameter Parse(string identity)
		{
			return new SharingPolicyIdParameter(identity);
		}
	}
}
