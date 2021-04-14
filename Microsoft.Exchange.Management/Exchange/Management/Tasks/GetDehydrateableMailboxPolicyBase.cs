using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class GetDehydrateableMailboxPolicyBase<T> : GetDeepSearchMailboxPolicyBase<DehydrateableMailboxPolicyIdParameter, T> where T : MailboxPolicy, new()
	{
	}
}
