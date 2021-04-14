using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[LocDescription(Strings.IDs.UninstallReplayServiceTask)]
	[Cmdlet("Uninstall", "MSExchangeReplService")]
	public sealed class UninstallReplService : ManageReplService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			base.UninstallEventManifest();
			base.RestoreDynamicPortRange();
			TaskLogger.LogExit();
		}
	}
}
