using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "MigrationWorkflowService")]
	[LocDescription(Strings.IDs.UninstallMigrationWorkflowServiceTask)]
	public sealed class UninstallMigrationWorkflowService : ManageMigrationWorkflowService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
