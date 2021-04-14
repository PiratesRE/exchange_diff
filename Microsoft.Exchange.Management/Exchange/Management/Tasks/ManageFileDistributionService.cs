using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageFileDistributionService : ManageService
	{
		protected ManageFileDistributionService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.FDServiceDisplayName;
			base.Description = Strings.FDServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeFDS.exe");
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "Microsoft.Exchange.Data.FileDistributionService.EventLog.dll");
			base.CategoryCount = 2;
			base.FirstFailureActionDelay = 5000U;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureActionsFlag = true;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = new string[]
			{
				ManageFileDistributionService.ActiveDirectoryTopologyService,
				"lanmanworkstation"
			};
			base.ServiceInstallContext = installContext;
			base.ServiceInstaller.AfterInstall += this.AfterInstallEventHandler;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeFDS";
			}
		}

		private void AfterInstallEventHandler(object sender, InstallEventArgs e)
		{
			base.LockdownServiceAccess();
		}

		private const string ServiceShortName = "MSExchangeFDS";

		private const string ServiceBinaryName = "MSExchangeFDS.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.Data.FileDistributionService.EventLog.dll";

		public const string WorkstationService = "lanmanworkstation";

		public static readonly string ActiveDirectoryTopologyService = "MSExchangeADTopology";
	}
}
