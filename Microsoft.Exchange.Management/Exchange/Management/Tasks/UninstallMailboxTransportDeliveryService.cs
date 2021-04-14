using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.UninstallMailboxTransportDeliveryServiceTask)]
	[Cmdlet("Uninstall", "DeliveryService")]
	public sealed class UninstallMailboxTransportDeliveryService : ManageMailboxTransportDeliveryService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
