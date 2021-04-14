using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallHealthManagerServiceTask)]
	[Cmdlet("Install", "HealthManagerService")]
	public class InstallHealthManagerService : ManageHealthManagerService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			base.RegisterProcessManagerEventLog();
			base.PersistManagedAvailabilityServersUsgSid();
			TaskLogger.LogExit();
		}
	}
}
