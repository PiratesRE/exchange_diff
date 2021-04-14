using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageMigrationWorkflowService : ManageService
	{
		protected ManageMigrationWorkflowService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.MigrationWorkflowServiceDisplayName;
			base.Description = Strings.MigrationWorkflowServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeMigrationWorkflow.exe");
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "Microsoft.Exchange.MigrationWorkflowService.EventLog.dll");
			base.CategoryCount = 2;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = ManageMigrationWorkflowService.ServicesDependencies;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeMigrationWorkflow";
			}
		}

		private const string ServiceShortName = "MSExchangeMigrationWorkflow";

		private const string ServiceBinaryName = "MSExchangeMigrationWorkflow.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.MigrationWorkflowService.EventLog.dll";

		private static readonly string[] ServicesDependencies = new string[]
		{
			"NetTcpPortSharing"
		};
	}
}
