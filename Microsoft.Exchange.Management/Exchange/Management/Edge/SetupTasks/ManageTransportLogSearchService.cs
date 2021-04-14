using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	public abstract class ManageTransportLogSearchService : ManageService
	{
		protected ManageTransportLogSearchService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.TransportLogSearchServiceDisplayName;
			base.Description = Strings.TransportLogSearchServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeTransportLogSearch.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = null;
			base.AddFirewallRule(new MSExchangeTransportLogSearchFirewallRule());
			base.AddFirewallRule(new MSExchangeTransportLogSearchRPCEPMapperFirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeTransportLogSearch";
			}
		}

		private const string ServiceShortName = "MSExchangeTransportLogSearch";

		private const string ServiceBinaryName = "MSExchangeTransportLogSearch.exe";
	}
}
