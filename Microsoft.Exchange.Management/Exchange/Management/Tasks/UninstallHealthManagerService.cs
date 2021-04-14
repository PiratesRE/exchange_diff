using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "HealthManagerService")]
	[LocDescription(Strings.IDs.UninstallHealthManagerServiceTask)]
	public class UninstallHealthManagerService : ManageHealthManagerService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			base.RemoveManagedAvailabilityServersUsgSidCache();
			TaskLogger.LogExit();
		}
	}
}
