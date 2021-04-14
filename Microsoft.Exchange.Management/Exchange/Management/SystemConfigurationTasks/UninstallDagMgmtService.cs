using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[LocDescription(Strings.IDs.UninstallDagMgmtServiceTask)]
	[Cmdlet("Uninstall", "DagMgmtService")]
	public sealed class UninstallDagMgmtService : ManageDagMgmtService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
