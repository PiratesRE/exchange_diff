using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "ADTopologyService")]
	[LocDescription(Strings.IDs.UninstallADTopologyServiceTask)]
	public class UninstallADTopologyService : ManageADTopologyService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
