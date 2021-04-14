using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageCentralAdminService : ManageService
	{
		protected ManageCentralAdminService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.CentralAdminServiceDisplayName;
			base.Description = Strings.CentralAdminServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Management.CentralAdmin.CentralAdminService.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.ServiceInstallContext = installContext;
			base.CategoryCount = 1;
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Management.CentralAdmin.CentralAdminServiceMsg.dll");
			base.ServicesDependedOn = null;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeCentralAdmin";
			}
		}

		protected const string ServiceShortName = "MSExchangeCentralAdmin";

		private const string ServiceBinaryName = "Microsoft.Exchange.Management.CentralAdmin.CentralAdminService.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.Management.CentralAdmin.CentralAdminServiceMsg.dll";
	}
}
