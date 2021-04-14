using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningAgentTasks
{
	[Cmdlet("Disable", "CmdletExtensionAgent", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class DisableCmdletExtensionAgent : FlipCmdletExtensionAgent
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableCmdletExtensionAgent(this.Identity.ToString());
			}
		}

		internal override bool FlipTo
		{
			get
			{
				return false;
			}
		}
	}
}
