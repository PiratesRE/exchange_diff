using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "ActiveSyncDeviceClass", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveActiveSyncDeviceClass : RemoveSystemConfigurationObjectTask<ActiveSyncDeviceClassIdParameter, ActiveSyncDeviceClass>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveActiveSyncDeviceClass(this.Identity.ToString());
			}
		}
	}
}
