using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[LocDescription(Strings.IDs.UninstallUmCallRouterTask)]
	[Cmdlet("Uninstall", "UMCallRouter")]
	public class UninstallUMCallRouter : UMCallRouterTask
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
