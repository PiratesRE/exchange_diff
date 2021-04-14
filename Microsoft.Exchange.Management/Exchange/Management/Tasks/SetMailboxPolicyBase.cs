using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class SetMailboxPolicyBase<T> : SetSystemConfigurationObjectTask<MailboxPolicyIdParameter, T> where T : MailboxPolicy, new()
	{
		protected IList<T> otherDefaultPolicies;

		protected bool updateOtherDefaultPolicies;
	}
}
