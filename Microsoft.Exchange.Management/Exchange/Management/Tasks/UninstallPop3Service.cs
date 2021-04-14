using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "Pop3Service")]
	[LocDescription(Strings.IDs.UninstallPop3ServiceTask)]
	public class UninstallPop3Service : ManagePop3Service
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.UninstallPopImapService();
			TaskLogger.LogExit();
		}
	}
}
