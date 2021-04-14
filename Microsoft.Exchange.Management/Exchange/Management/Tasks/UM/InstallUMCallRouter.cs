using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Install", "UMCallRouter")]
	[LocDescription(Strings.IDs.InstallUmCallRouterTask)]
	public class InstallUMCallRouter : UMCallRouterTask
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.ReservePorts(5060, 2);
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
