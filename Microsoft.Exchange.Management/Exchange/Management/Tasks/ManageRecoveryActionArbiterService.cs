using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageRecoveryActionArbiterService : ManageService
	{
		protected ManageRecoveryActionArbiterService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.RecoveryActionArbiterServiceDisplayName;
			base.Description = Strings.RecoveryActionArbiterServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Forefront.RecoveryActionArbiter.RaaService.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.ServiceInstallContext = installContext;
			base.CategoryCount = 1;
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Forefront.RecoveryActionArbiter.RaaServiceMsg.dll");
			List<string> list = new List<string>
			{
				"NetTcpPortSharing"
			};
			base.ServicesDependedOn = list.ToArray();
		}

		protected override string Name
		{
			get
			{
				return "FfoRecoveryActionArbiter";
			}
		}

		protected const string ServiceShortName = "FfoRecoveryActionArbiter";

		private const string ServiceBinaryName = "Microsoft.Forefront.RecoveryActionArbiter.RaaService.exe";

		private const string EventLogBinaryName = "Microsoft.Forefront.RecoveryActionArbiter.RaaServiceMsg.dll";
	}
}
