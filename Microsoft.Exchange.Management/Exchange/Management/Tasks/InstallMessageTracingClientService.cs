using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "MessageTracingClientService")]
	[LocDescription(Strings.IDs.InstallMessageTracingClientServiceTask)]
	public class InstallMessageTracingClientService : ManageMessageTracingClientService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
