using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallPop3BeServiceTask)]
	[Cmdlet("Install", "Pop3BeService")]
	public class InstallPop3BeService : ManagePop3BeService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InstallPopImapService();
			TaskLogger.LogExit();
		}
	}
}
