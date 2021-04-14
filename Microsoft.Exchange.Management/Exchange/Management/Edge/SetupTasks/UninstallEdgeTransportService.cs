using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[LocDescription(Strings.IDs.UninstallEdgeTransportServiceTask)]
	[Cmdlet("Uninstall", "EdgeTransportService")]
	public class UninstallEdgeTransportService : ManageEdgeTransportService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
