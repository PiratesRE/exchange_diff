using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Enable", "UMMailbox", SupportsShouldProcess = true)]
	public sealed class EnableUMMailbox : EnableUMMailboxBase<MailboxIdParameter>
	{
	}
}
