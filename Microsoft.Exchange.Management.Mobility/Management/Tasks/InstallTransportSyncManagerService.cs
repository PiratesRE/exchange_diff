using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallTransportSyncManagerServiceTask)]
	[Cmdlet("Install", "TransportSyncManagerService")]
	public class InstallTransportSyncManagerService : ManageTransportSyncManagerService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
