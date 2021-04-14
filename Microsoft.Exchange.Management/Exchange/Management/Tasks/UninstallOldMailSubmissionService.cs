using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "OldMailSubmissionService")]
	[LocDescription(Strings.IDs.UninstallOldMailSubmissionServiceTask)]
	public class UninstallOldMailSubmissionService : ManageOldMailSubmissionService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
