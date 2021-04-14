using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "MonitoringCorrelationService")]
	[LocDescription(Strings.IDs.UninstallMonitoringCorrelationServiceTask)]
	public class UninstallMonitoringCorrelationService : ManageMonitoringCorrelationService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
