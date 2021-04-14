using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DehydrateableMailboxPolicyIdParameter : ADIdParameter, IIdentityParameter
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		public DehydrateableMailboxPolicyIdParameter(string rawString) : base(rawString)
		{
		}

		public DehydrateableMailboxPolicyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public DehydrateableMailboxPolicyIdParameter(MailboxPolicy policy) : base(policy.Id)
		{
		}

		public DehydrateableMailboxPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public DehydrateableMailboxPolicyIdParameter()
		{
		}

		public static DehydrateableMailboxPolicyIdParameter Parse(string rawString)
		{
			return new DehydrateableMailboxPolicyIdParameter(rawString);
		}
	}
}
