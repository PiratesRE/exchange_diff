using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ClientAccessRuleIdParameter : ADIdParameter
	{
		public ClientAccessRuleIdParameter()
		{
		}

		public ClientAccessRuleIdParameter(string identity) : base(identity)
		{
		}

		public ClientAccessRuleIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ClientAccessRuleIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ClientAccessRuleIdParameter Parse(string identity)
		{
			return new ClientAccessRuleIdParameter(identity);
		}
	}
}
