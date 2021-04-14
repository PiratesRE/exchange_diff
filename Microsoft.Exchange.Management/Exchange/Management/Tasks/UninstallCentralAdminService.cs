using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "CentralAdminService")]
	[LocDescription(Strings.IDs.UninstallCentralAdminServiceTask)]
	public class UninstallCentralAdminService : ManageCentralAdminService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
