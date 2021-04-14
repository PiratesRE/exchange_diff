using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[LocDescription(Strings.IDs.UninstallOldEdgeTransportServiceTask)]
	[Cmdlet("Uninstall", "OldEdgeTransportService")]
	public class UninstallOldEdgeTransportService : ManageEdgeTransportService
	{
		protected override string Name
		{
			get
			{
				return "EdgeTransportSvc";
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}

		private const string ServiceShortName = "EdgeTransportSvc";
	}
}
