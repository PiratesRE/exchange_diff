using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.UninstallPop3BeServiceTask)]
	[Cmdlet("Uninstall", "Pop3BeService")]
	public class UninstallPop3BeService : ManagePop3BeService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.UninstallPopImapService();
			TaskLogger.LogExit();
		}
	}
}
