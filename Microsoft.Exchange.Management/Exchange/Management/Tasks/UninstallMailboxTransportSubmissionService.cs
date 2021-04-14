using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.UninstallMailboxTransportSubmissionServiceTask)]
	[Cmdlet("Uninstall", "SubmissionService")]
	public sealed class UninstallMailboxTransportSubmissionService : ManageMailboxTransportSubmissionService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
