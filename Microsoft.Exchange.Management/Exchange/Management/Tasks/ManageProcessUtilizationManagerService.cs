using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageProcessUtilizationManagerService : ManageService
	{
		protected ManageProcessUtilizationManagerService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.ProcessUtilizationManagerServiceDisplayName;
			base.Description = Strings.ProcessUtilizationManagerServiceDescription;
			string binPath = ConfigurationContext.Setup.BinPath;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(binPath, "Microsoft.Exchange.ProcessUtilizationManager.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 60000U;
			base.FailureResetPeriod = 3600U;
			base.FailureActionsFlag = true;
			base.ServiceInstallContext = installContext;
			base.CategoryCount = 2;
			base.ServicesDependedOn = null;
			base.CategoryCount = 2;
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.ProcessUtilizationManager.EventLog.dll");
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeProcessUtilizationManager";
			}
		}

		protected const string ServiceShortName = "MSExchangeProcessUtilizationManager";

		private const string ServiceBinaryName = "Microsoft.Exchange.ProcessUtilizationManager.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.ProcessUtilizationManager.EventLog.dll";
	}
}
