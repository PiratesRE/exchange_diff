using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Audit;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[Cmdlet("Install", "Audit")]
	public sealed class InstallAuditTask : Task
	{
		protected override void InternalValidate()
		{
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string installPath = ConfigurationContext.Setup.InstallPath;
			try
			{
				this.InstallMPAuditLog(installPath);
			}
			catch (InvalidOperationException)
			{
				EventSourceInstaller.UninstallSecurityEventSource("MSExchange Messaging Policies");
				this.InstallMPAuditLog(installPath);
			}
			TaskLogger.LogExit();
		}

		private void InstallMPAuditLog(string path)
		{
			EventSourceInstaller.InstallSecurityEventSource("MSExchange Messaging Policies", Path.Combine(path, "bin\\RulesAuditMsg.DLL"), null, null, Path.Combine(path, "bin\\EdgeTransport.exe"), false);
		}
	}
}
