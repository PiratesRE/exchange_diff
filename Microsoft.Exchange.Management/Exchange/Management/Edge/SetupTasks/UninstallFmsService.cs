using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[Cmdlet("Uninstall", "FmsService")]
	[LocDescription(Strings.IDs.UninstallFmsServiceTask)]
	public class UninstallFmsService : ManageFmsService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
