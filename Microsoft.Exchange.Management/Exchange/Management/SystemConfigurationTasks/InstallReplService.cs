using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[LocDescription(Strings.IDs.InstallReplayServiceTask)]
	[Cmdlet("Install", "MSExchangeReplService")]
	public class InstallReplService : ManageReplService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InstallEventManifest();
			base.RegisterDefaultLogCopierPort();
			base.RegisterDefaultHighAvailabilityWebServicePort();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
