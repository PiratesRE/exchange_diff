using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageMonitoringService : ManageService
	{
		public ManageMonitoringService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Manual;
			base.DisplayName = Strings.MonitoringServiceDisplayName;
			base.Description = Strings.MonitoringServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Monitoring.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = null;
			base.AddFirewallRule(new MSExchangeMonitoringFirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeMonitoring";
			}
		}

		private const string ServiceShortName = "MSExchangeMonitoring";

		private const string ServiceBinaryName = "Microsoft.Exchange.Monitoring.exe";
	}
}
