using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallForwardSyncServiceTask)]
	[Cmdlet("Install", "ForwardSync2Service")]
	public class InstallForwardSync2Service : ManageForwardSync2Service
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
