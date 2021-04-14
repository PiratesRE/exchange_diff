using System;
using System.Configuration.Install;
using System.IO;
using System.Management.Automation;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.UninstallOldAssistantsServiceTask)]
	[Cmdlet("Uninstall", "OldAssistantsService")]
	public class UninstallOldAssistantsService : ManageService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}

		public UninstallOldAssistantsService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = "Microsoft Exchange Mailbox Assistants";
			base.Description = Strings.AssistantsServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.InfoWorker.Assistants.exe");
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "Microsoft.Exchange.InfoWorker.Eventlog.dll");
			base.CategoryCount = 10;
			base.ServiceInstallContext = installContext;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeMA";
			}
		}

		private const string ServiceShortName = "MSExchangeMA";

		private const string ServiceDisplayName = "Microsoft Exchange Mailbox Assistants";

		private const string ServiceBinaryName = "Microsoft.Exchange.InfoWorker.Assistants.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.InfoWorker.Eventlog.dll";
	}
}
