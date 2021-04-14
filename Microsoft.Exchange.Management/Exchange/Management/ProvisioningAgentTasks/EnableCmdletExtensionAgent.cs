using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningAgentTasks
{
	[Cmdlet("Enable", "CmdletExtensionAgent", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class EnableCmdletExtensionAgent : FlipCmdletExtensionAgent
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableCmdletExtensionAgent(this.Identity.ToString());
			}
		}

		internal override bool FlipTo
		{
			get
			{
				return true;
			}
		}
	}
}
