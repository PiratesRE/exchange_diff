using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.PolicyNudges
{
	[Cmdlet("Remove", "PolicyTipConfig", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemovePolicyTipConfig : RemoveSystemConfigurationObjectTask<PolicyTipConfigIdParameter, PolicyTipMessageConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemovePolicyTipConfig(this.Identity.ToString());
			}
		}
	}
}
