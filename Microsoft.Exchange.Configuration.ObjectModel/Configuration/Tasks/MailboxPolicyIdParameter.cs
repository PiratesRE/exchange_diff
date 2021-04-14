using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailboxPolicyIdParameter : ADIdParameter, IIdentityParameter
	{
		public MailboxPolicyIdParameter(string rawString) : base(rawString)
		{
		}

		public MailboxPolicyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailboxPolicyIdParameter(MailboxPolicy policy) : base(policy.Id)
		{
		}

		public MailboxPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public MailboxPolicyIdParameter()
		{
		}

		public static MailboxPolicyIdParameter Parse(string rawString)
		{
			return new MailboxPolicyIdParameter(rawString);
		}
	}
}
