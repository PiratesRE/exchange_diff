using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "OwaMailboxPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetOwaMailboxPolicy : GetMailboxPolicyBase<OwaMailboxPolicy>
	{
	}
}
