using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallMailboxTransportSubmissionServiceTask)]
	[Cmdlet("Install", "SubmissionService")]
	public sealed class InstallMailboxTransportSubmissionService : ManageMailboxTransportSubmissionService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
