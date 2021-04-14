using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "Imap4Service")]
	[LocDescription(Strings.IDs.InstallImap4ServiceTask)]
	public class InstallImap4Service : ManageImap4Service
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InstallPopImapService();
			base.ReservePort(143);
			base.ReservePort(993);
			TaskLogger.LogExit();
		}
	}
}
