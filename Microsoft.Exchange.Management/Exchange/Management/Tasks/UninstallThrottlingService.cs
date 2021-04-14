using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "ThrottlingService")]
	[LocDescription(Strings.IDs.UninstallThrottlingServiceTask)]
	public sealed class UninstallThrottlingService : ManageThrottlingService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
