using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManagePopImapService : ManageService
	{
		protected ManagePopImapService()
		{
			base.Account = ServiceAccount.NetworkService;
			base.StartMode = ServiceStartMode.Manual;
			base.DisplayName = this.ServiceDisplayName;
			base.Description = this.ServiceDescription;
			this.installPath = Path.Combine(ConfigurationContext.Setup.InstallPath, this.RelativeInstallPath);
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(this.installPath, this.ServiceFile);
			base.ServiceInstallContext = installContext;
			base.EventMessageFile = Path.Combine(this.installPath, this.WorkingProcessEventMessageFile);
			base.CategoryCount = 1;
		}

		protected abstract string RelativeInstallPath { get; }

		protected string RelativeEventLogFilePath
		{
			get
			{
				return "Bin";
			}
		}

		protected abstract string ServiceDisplayName { get; }

		protected abstract string ServiceDescription { get; }

		protected abstract string ServiceFile { get; }

		protected string ServiceEventMessageFile
		{
			get
			{
				return "Microsoft.Exchange.Common.ProcessManagerMsg.dll";
			}
		}

		protected abstract string ServiceCategoryName { get; }

		protected abstract string WorkingProcessFile { get; }

		protected abstract string WorkingProcessEventMessageFile { get; }

		protected void InstallPopImapService()
		{
			base.Install();
			this.RegisterServiceEventMessageFile();
		}

		protected void UninstallPopImapService()
		{
			base.Uninstall();
			this.UnregisterServiceEventMessageFile();
		}

		protected void ReservePort(int portNumber)
		{
			TcpListener.CreatePersistentTcpPortReservation((ushort)portNumber, 1);
		}

		protected void RegisterServiceEventMessageFile()
		{
			string value = Path.Combine(Path.Combine(ConfigurationContext.Setup.InstallPath, this.RelativeEventLogFilePath), this.ServiceEventMessageFile);
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(Path.Combine(ManageService.eventLogRegPath, this.ServiceCategoryName)))
			{
				registryKey.SetValue(ManageService.eventMessageFileSubKeyName, value);
				registryKey.SetValue(ManageService.categoryMessageFileSubKeyName, value);
				registryKey.SetValue(ManageService.categoryCountSubKeyName, 1);
				registryKey.SetValue(ManageService.typesSupportedSubKeyName, 7);
			}
		}

		protected void UnregisterServiceEventMessageFile()
		{
			Registry.LocalMachine.DeleteSubKey(Path.Combine(ManageService.eventLogRegPath, this.ServiceCategoryName), false);
		}

		protected string installPath;

		public bool ForceFailure;
	}
}
