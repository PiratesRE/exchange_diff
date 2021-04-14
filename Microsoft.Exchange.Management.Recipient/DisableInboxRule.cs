using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Disable", "InboxRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableInboxRule : EnableDisableInboxRuleBase
	{
		public DisableInboxRule() : base(false)
		{
		}
	}
}
