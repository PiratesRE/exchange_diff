using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[Cmdlet("Uninstall", "AntispamUpdateService")]
	[LocDescription(Strings.IDs.UninstallAntispamUpdateServiceTask)]
	public class UninstallAntispamUpdateService : ManageAntispamUpdateService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
