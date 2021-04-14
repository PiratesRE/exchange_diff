using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "MessageTracingClientService")]
	[LocDescription(Strings.IDs.UninstallMessageTracingClientServiceTask)]
	public class UninstallMessageTracingClientService : ManageMessageTracingClientService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
