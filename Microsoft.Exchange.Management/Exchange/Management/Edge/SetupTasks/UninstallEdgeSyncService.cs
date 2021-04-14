using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[Cmdlet("Uninstall", "EdgeSyncService")]
	[LocDescription(Strings.IDs.UninstallEdgeSyncServiceTask)]
	public class UninstallEdgeSyncService : ManageEdgeSyncService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
