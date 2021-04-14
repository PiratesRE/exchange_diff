using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageMonitoringCorrelationService : ManageService
	{
		public ManageMonitoringCorrelationService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.MonitoringCorrelationServiceDisplayName;
			base.Description = Strings.MonitoringCorrelationServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Monitoring.CorrelationEngine.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = null;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeMonitoringCorrelation";
			}
		}

		private const string ServiceShortName = "MSExchangeMonitoringCorrelation";

		private const string ServiceBinaryName = "Microsoft.Exchange.Monitoring.CorrelationEngine.exe";
	}
}
