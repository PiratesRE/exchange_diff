using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "ActiveSyncDeviceAccessRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveActiveSyncDeviceAccessRule : RemoveSystemConfigurationObjectTask<ActiveSyncDeviceAccessRuleIdParameter, ActiveSyncDeviceAccessRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveActiveSyncDeviceAccessRule(this.Identity.ToString());
			}
		}
	}
}
