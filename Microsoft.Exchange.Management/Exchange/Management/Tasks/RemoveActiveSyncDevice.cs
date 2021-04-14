using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "ActiveSyncDevice", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveActiveSyncDevice : RemoveMobileDevice
	{
		protected override void InternalValidate()
		{
			this.WriteWarning(Strings.WarningCmdletIsDeprecated("Remove-ActiveSyncDevice", "Remove-MobileDevice"));
			base.InternalValidate();
			if ((base.DataObject != null && base.DataObject.ClientType != MobileClientType.EAS) || this.Identity.ClientType != MobileClientType.EAS)
			{
				base.WriteError(new LocalizedException(Strings.InvalidIdentityTypeForRemoveCmdletException(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}
