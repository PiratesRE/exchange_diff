using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Audit;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[Cmdlet("Uninstall", "Audit")]
	public sealed class UnInstallAuditTask : Task
	{
		protected override void InternalValidate()
		{
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			EventSourceInstaller.UninstallSecurityEventSource("MSExchange Messaging Policies");
			TaskLogger.LogExit();
		}
	}
}
