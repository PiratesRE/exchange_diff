using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "ForwardSyncService")]
	[LocDescription(Strings.IDs.UninstallForwardSyncServiceTask)]
	public class UninstallForwardSyncService : ManageCentralAdminService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
