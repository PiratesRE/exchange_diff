using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallNotificationsBrokerServiceTask)]
	[Cmdlet("Install", "NotificationsBrokerService")]
	public sealed class InstallNotificationsBrokerService : ManageNotificationsBrokerService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
