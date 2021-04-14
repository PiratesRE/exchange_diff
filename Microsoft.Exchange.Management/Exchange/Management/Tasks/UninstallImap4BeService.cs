using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "Imap4BeService")]
	[LocDescription(Strings.IDs.UninstallImap4BeServiceTask)]
	public class UninstallImap4BeService : ManageImap4BeService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.UninstallPopImapService();
			TaskLogger.LogExit();
		}
	}
}
