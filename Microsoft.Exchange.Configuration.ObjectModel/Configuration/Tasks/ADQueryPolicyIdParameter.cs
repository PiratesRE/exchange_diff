using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ADQueryPolicyIdParameter : ADIdParameter
	{
		public ADQueryPolicyIdParameter()
		{
		}

		public ADQueryPolicyIdParameter(string identity) : base(identity)
		{
		}

		public ADQueryPolicyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ADQueryPolicyIdParameter(ADQueryPolicy policy) : base(policy.Id)
		{
		}

		public ADQueryPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ADQueryPolicyIdParameter Parse(string identity)
		{
			return new ADQueryPolicyIdParameter(identity);
		}
	}
}
