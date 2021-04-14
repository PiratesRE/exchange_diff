using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Disable", "UMMailbox", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableUMMailbox : DisableUMMailboxBase<MailboxIdParameter>
	{
	}
}
