using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[LocDescription(Strings.IDs.InstallEdgeTransportServiceTask)]
	[Cmdlet("Install", "EdgeSyncService")]
	public class InstallEdgeSyncService : ManageEdgeSyncService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
