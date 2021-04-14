using System;
using System.Configuration.Install;
using System.IO;
using System.Management.Automation;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	public abstract class ManageEdgeCredentialService : ManageService
	{
		protected ManageEdgeCredentialService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.EdgeCredentialServiceDisplayName;
			base.Description = Strings.EdgeCredentialServiceDescription;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.EdgeCredentialSvc.exe");
			base.ServiceInstallContext = installContext;
		}

		[Parameter]
		public new string[] ServicesDependedOn
		{
			get
			{
				return base.ServicesDependedOn;
			}
			set
			{
				base.ServicesDependedOn = value;
			}
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeEdgeCredential";
			}
		}
	}
}
