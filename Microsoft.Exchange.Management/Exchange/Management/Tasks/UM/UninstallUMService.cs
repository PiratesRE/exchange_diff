using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[LocDescription(Strings.IDs.UninstallUmServiceTask)]
	[Cmdlet("Uninstall", "UMService")]
	public class UninstallUMService : UMServiceTask
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
