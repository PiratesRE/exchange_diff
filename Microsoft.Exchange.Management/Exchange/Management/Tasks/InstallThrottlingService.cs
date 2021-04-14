using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallThrottlingServiceTask)]
	[Cmdlet("Install", "ThrottlingService")]
	public sealed class InstallThrottlingService : ManageThrottlingService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
