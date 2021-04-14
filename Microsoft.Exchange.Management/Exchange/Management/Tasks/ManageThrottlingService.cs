using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageThrottlingService : ManageService
	{
		public ManageThrottlingService()
		{
			base.Account = ServiceAccount.NetworkService;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.ThrottlingServiceDisplayName;
			base.Description = Strings.ThrottlingServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeThrottling.exe");
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "Microsoft.Exchange.Data.ThrottlingService.EventLog.dll");
			base.CategoryCount = 1;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.AddFirewallRule(new MSExchangeThrottlingRPCEPMapFirewallRule());
			base.AddFirewallRule(new MSExchangeThrottlingRPCFirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeThrottling";
			}
		}

		private const string ServiceShortName = "MSExchangeThrottling";

		private const string ServiceBinaryName = "MSExchangeThrottling.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.Data.ThrottlingService.EventLog.dll";
	}
}
