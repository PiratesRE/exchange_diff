using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "ForwardSyncService")]
	[LocDescription(Strings.IDs.InstallForwardSyncServiceTask)]
	public class InstallForwardSyncService : ManageForwardSyncService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
