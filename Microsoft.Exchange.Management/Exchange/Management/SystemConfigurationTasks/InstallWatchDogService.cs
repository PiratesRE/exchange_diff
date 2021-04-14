using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[LocDescription(Strings.IDs.InstallWatchDogServiceTask)]
	[Cmdlet("Install", "WatchDogService")]
	public sealed class InstallWatchDogService : ManageWatchDogService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
