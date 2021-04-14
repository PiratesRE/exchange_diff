using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class GetMailboxPolicyBase<T> : GetDeepSearchMailboxPolicyBase<MailboxPolicyIdParameter, T> where T : MailboxPolicy, new()
	{
	}
}
