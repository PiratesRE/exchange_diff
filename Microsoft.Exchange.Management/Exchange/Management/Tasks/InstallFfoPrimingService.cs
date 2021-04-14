using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "FfoPrimingService")]
	[LocDescription(Strings.IDs.InstallFfoPrimingServiceTask)]
	public sealed class InstallFfoPrimingService : ManageFfoPrimingService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
