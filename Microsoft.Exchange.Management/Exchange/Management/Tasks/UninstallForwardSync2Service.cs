using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.UninstallForwardSyncServiceTask)]
	[Cmdlet("Uninstall", "ForwardSync2Service")]
	public class UninstallForwardSync2Service : ManageCentralAdminService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
