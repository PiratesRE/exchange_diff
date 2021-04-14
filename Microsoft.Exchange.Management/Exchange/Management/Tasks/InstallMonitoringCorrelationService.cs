using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallMonitoringCorrelationServiceTask)]
	[Cmdlet("Install", "MonitoringCorrelationService")]
	public class InstallMonitoringCorrelationService : ManageMonitoringCorrelationService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
