using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "ActiveSyncDeviceAutoblockThreshold", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetActiveSyncDeviceAutoblockThreshold : SetSystemConfigurationObjectTask<ActiveSyncDeviceAutoblockThresholdIdParameter, ActiveSyncDeviceAutoblockThreshold>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetActiveSyncDeviceAutoblockThreshold(this.Identity.ToString());
			}
		}
	}
}
