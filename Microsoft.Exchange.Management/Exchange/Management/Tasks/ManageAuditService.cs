using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageAuditService : ManageService
	{
		protected ManageAuditService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = "Microsoft Exchange Audit Service";
			base.Description = "Exchange security audits.";
			string binPath = ConfigurationContext.Setup.BinPath;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(binPath, "Microsoft.Exchange.Audit.Service.exe");
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
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeAS";
			}
		}

		protected const string ServiceShortName = "MSExchangeAS";

		private const string ServiceBinaryName = "Microsoft.Exchange.Audit.Service.exe";
	}
}
