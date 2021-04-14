using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Clear", "ActiveSyncDevice", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class ClearActiveSyncDevice : ClearMobileDevice
	{
		protected override void InternalValidate()
		{
			this.WriteWarning(Strings.WarningCmdletIsDeprecated("Clear-ActiveSyncDevice", "Clear-MobileDevice"));
			base.InternalValidate();
			if (this.DataObject.ClientType != MobileClientType.EAS)
			{
				base.WriteError(new LocalizedException(Strings.InvalidIdentityTypeForClearCmdletException(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}
