using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageAssistantsService : ManageService
	{
		public ManageAssistantsService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.AssistantsServiceDisplayName;
			base.Description = Strings.AssistantsServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeMailboxAssistants.exe");
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "Microsoft.Exchange.InfoWorker.Eventlog.dll");
			base.CategoryCount = 19;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.AddFirewallRule(new MSExchangeMailboxAssistantsRPCFirewallRule());
			base.AddFirewallRule(new MSExchangeMailboxAssistantsRPCEPMapFirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeMailboxAssistants";
			}
		}

		private const string ServiceShortName = "MSExchangeMailboxAssistants";

		private const string ServiceBinaryName = "MSExchangeMailboxAssistants.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.InfoWorker.Eventlog.dll";
	}
}
