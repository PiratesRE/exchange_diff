using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "MailboxReplicationService")]
	[LocDescription(Strings.IDs.UninstallMailboxReplicationServiceTask)]
	public sealed class UninstallMailboxReplicationService : ManageMailboxReplicationService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
