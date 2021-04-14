using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RemoteAccountPolicyIdParameter : ADIdParameter
	{
		public RemoteAccountPolicyIdParameter()
		{
		}

		public RemoteAccountPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity.Identity)
		{
		}

		public RemoteAccountPolicyIdParameter(RemoteAccountPolicy remoteAccountPolicy) : base(remoteAccountPolicy.Id)
		{
		}

		public RemoteAccountPolicyIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		protected RemoteAccountPolicyIdParameter(string identity) : base(identity)
		{
		}

		public static RemoteAccountPolicyIdParameter Parse(string identity)
		{
			return new RemoteAccountPolicyIdParameter(identity);
		}
	}
}
