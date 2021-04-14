using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageExchangeFileUploadService : ManageService
	{
		protected ManageExchangeFileUploadService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.FileUploadServiceDisplayName;
			base.Description = Strings.FileUploadServiceDescription;
			string path = Path.Combine(ConfigurationContext.Setup.InstallPath, this.RelativeInstallPath);
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(path, "Microsoft.Exchange.Management.DataMining.ExchangeFileUpload.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.FailureActionsFlag = true;
			base.ServiceInstallContext = installContext;
			base.CategoryCount = 2;
			base.ServicesDependedOn = null;
			foreach (object obj in base.ServiceInstaller.Installers)
			{
				EventLogInstaller eventLogInstaller = obj as EventLogInstaller;
				if (eventLogInstaller != null)
				{
					eventLogInstaller.Source = "MSExchange DataMining";
					eventLogInstaller.Log = "Application";
				}
			}
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeFileUpload";
			}
		}

		protected string RelativeInstallPath
		{
			get
			{
				return "Datacenter\\DataMining\\Cosmos";
			}
		}

		protected const string ServiceShortName = "MSExchangeFileUpload";

		private const string ServiceBinaryName = "Microsoft.Exchange.Management.DataMining.ExchangeFileUpload.exe";

		private const string EventLogName = "Application";

		private const string EventLogSourceName = "MSExchange DataMining";
	}
}
