using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageADTopologyService : ManageService
	{
		protected ManageADTopologyService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.ADTopologyServiceDisplayName;
			base.Description = Strings.ADTopologyServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Directory.TopologyService.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			List<string> list = new List<string>
			{
				"NetTcpPortSharing"
			};
			base.ServicesDependedOn = list.ToArray();
			base.AddFirewallRule(new MSExchangeADTopologyWCFFirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeADTopology";
			}
		}

		protected const string ServiceShortName = "MSExchangeADTopology";

		private const string ServiceBinaryName = "Microsoft.Exchange.Directory.TopologyService.exe";

		private const string EventLogBinaryName = "";
	}
}
