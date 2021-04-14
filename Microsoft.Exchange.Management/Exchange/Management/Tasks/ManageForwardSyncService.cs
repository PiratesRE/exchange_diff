using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageForwardSyncService : ManageService
	{
		protected ManageForwardSyncService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.ForwardSyncServiceDisplayName;
			base.Description = Strings.ForwardSyncServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Management.ForwardSync.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = new string[]
			{
				ManagedServiceName.ActiveDirectoryTopologyService
			};
			foreach (object obj in base.ServiceInstaller.Installers)
			{
				EventLogInstaller eventLogInstaller = obj as EventLogInstaller;
				if (eventLogInstaller != null)
				{
					eventLogInstaller.Source = "MSExchangeForwardSync";
					eventLogInstaller.Log = "ForwardSync";
				}
			}
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeForwardSync";
			}
		}

		protected const string ServiceShortName = "MSExchangeForwardSync";

		private const string ServiceBinaryName = "Microsoft.Exchange.Management.ForwardSync.exe";

		private const string EventLogName = "ForwardSync";

		private const string EventLogSourceName = "MSExchangeForwardSync";
	}
}
