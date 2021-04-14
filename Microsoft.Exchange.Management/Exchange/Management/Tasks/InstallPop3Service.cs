using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallPop3ServiceTask)]
	[Cmdlet("Install", "Pop3Service")]
	public class InstallPop3Service : ManagePop3Service
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InstallPopImapService();
			base.ReservePort(110);
			base.ReservePort(995);
			TaskLogger.LogExit();
		}
	}
}
