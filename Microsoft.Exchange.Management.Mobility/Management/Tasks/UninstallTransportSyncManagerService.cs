using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "TransportSyncManagerService")]
	[LocDescription(Strings.IDs.UninstallTransportSyncManagerServiceTask)]
	public class UninstallTransportSyncManagerService : ManageTransportSyncManagerService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
