using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.UninstallFfoPrimingServiceTask)]
	[Cmdlet("Uninstall", "FfoPrimingService")]
	public sealed class UninstallFfoPrimingService : ManageFfoPrimingService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
