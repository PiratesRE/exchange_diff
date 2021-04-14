using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallMailboxTransportDeliveryServiceTask)]
	[Cmdlet("Install", "DeliveryService")]
	public sealed class InstallMailboxTransportDeliveryService : ManageMailboxTransportDeliveryService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
