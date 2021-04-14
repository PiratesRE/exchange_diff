using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "MigrationWorkflowService")]
	[LocDescription(Strings.IDs.InstallMigrationWorkflowServiceTask)]
	public sealed class InstallMigrationWorkflowService : ManageMigrationWorkflowService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
