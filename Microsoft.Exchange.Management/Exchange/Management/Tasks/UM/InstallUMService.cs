using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Install", "UMService")]
	[LocDescription(Strings.IDs.InstallUmServiceTask)]
	public class InstallUMService : UMServiceTask
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.ReservePorts(5062, 7);
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
