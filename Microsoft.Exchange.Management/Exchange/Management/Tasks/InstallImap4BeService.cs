using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallImap4BeServiceTask)]
	[Cmdlet("Install", "Imap4BeService")]
	public class InstallImap4BeService : ManageImap4BeService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InstallPopImapService();
			TaskLogger.LogExit();
		}
	}
}
