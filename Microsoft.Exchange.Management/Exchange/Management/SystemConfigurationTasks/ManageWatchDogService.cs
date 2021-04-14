using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class ManageWatchDogService : ManageService
	{
		protected ManageWatchDogService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.WatchDogServiceDisplayName;
			base.Description = Strings.WatchDogServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeWatchDog.exe");
			base.ServiceInstallContext = installContext;
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.BinPath, "clusmsg.dll");
			base.CategoryCount = 6;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstaller.AfterInstall += this.AfterInstallEventHandler;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeWatchDog";
			}
		}

		private void AfterInstallEventHandler(object sender, InstallEventArgs e)
		{
			base.LockdownServiceAccess();
		}

		private const string ServiceShortName = "MSExchangeWatchDog";

		private const string ServiceBinaryName = "MSExchangeWatchDog.exe";

		private const string EventLogBinaryName = "clusmsg.dll";
	}
}
