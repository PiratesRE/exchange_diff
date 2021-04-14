using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Enable", "UMMailboxPlan", SupportsShouldProcess = true)]
	public sealed class EnableUMMailboxPlan : EnableUMMailboxBase<MailboxPlanIdParameter>
	{
		protected override bool ShouldSavePin
		{
			get
			{
				return false;
			}
		}

		protected override bool ShouldSubmitWelcomeMessage
		{
			get
			{
				return false;
			}
		}

		protected override bool ShouldInitUMMailbox
		{
			get
			{
				return false;
			}
		}

		protected override void InternalProcessRecord()
		{
			this.DataObject.UMProvisioningRequested = true;
			base.InternalProcessRecord();
		}
	}
}
