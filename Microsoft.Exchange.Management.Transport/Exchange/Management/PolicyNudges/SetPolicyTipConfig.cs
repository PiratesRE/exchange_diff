using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.PolicyNudges
{
	[Cmdlet("Set", "PolicyTipConfig", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetPolicyTipConfig : SetSystemConfigurationObjectTask<PolicyTipConfigIdParameter, PolicyTipMessageConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetPolicyTipConfig(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.Action == PolicyTipMessageConfigAction.Url && !NewPolicyTipConfig.IsAbsoluteUri(base.DynamicParametersInstance.Value))
			{
				base.WriteError(new NewPolicyTipConfigInvalidUrlException(), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}
	}
}
