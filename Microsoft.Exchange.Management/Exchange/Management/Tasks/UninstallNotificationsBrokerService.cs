using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "NotificationsBrokerService")]
	[LocDescription(Strings.IDs.UninstallNotificationsBrokerServiceTask)]
	public sealed class UninstallNotificationsBrokerService : ManageNotificationsBrokerService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
