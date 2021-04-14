using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.UninstallMomConnectorServiceTask)]
	[Cmdlet("Uninstall", "MomConnectorService")]
	public class UninstallMomConnectorService : ManageMomConnectorService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
