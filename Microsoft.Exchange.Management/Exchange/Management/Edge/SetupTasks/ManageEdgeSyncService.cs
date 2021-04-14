using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	public abstract class ManageEdgeSyncService : ManageService
	{
		protected ManageEdgeSyncService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.EdgeSyncServiceDisplayName;
			base.Description = Strings.EdgeSyncServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.EdgeSyncSvc.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.AddFirewallRule(new MSExchangeEdgesyncRPCRule());
			base.AddFirewallRule(new MSExchangeEdgesyncRPCEPMapRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeEdgeSync";
			}
		}

		private const string ServiceShortName = "MSExchangeEdgeSync";

		private const string ServiceBinaryName = "Microsoft.Exchange.EdgeSyncSvc.exe";
	}
}
