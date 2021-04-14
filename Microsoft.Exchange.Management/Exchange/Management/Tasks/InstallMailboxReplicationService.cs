using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "MailboxReplicationService")]
	[LocDescription(Strings.IDs.InstallMailboxReplicationServiceTask)]
	public sealed class InstallMailboxReplicationService : ManageMailboxReplicationService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
