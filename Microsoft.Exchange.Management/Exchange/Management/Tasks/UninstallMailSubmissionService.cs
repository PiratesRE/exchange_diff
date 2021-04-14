using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.UninstallMailSubmissionServiceTask)]
	[Cmdlet("Uninstall", "MailSubmissionService")]
	public class UninstallMailSubmissionService : ManageMailSubmissionService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
