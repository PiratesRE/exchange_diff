using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageServiceHost : ManageService
	{
		protected ManageServiceHost()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.ServiceHostDisplayName;
			base.Description = Strings.ServiceHostDescription;
			string path = Path.Combine(ConfigurationContext.Setup.InstallPath, this.RelativeInstallPath);
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(path, "Microsoft.Exchange.ServiceHost.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.CategoryCount = 1;
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "Microsoft.Exchange.ServiceHost.EventLog.dll");
			base.ServiceInstaller.AfterInstall += this.AfterInstallEventHandler;
			base.AddFirewallRule(new MSExchangeServiceHostRPCFirewallRule());
			base.AddFirewallRule(new MSExchangeServiceHostRPCEPMapFirewallRule());
		}

		private void AfterInstallEventHandler(object sender, InstallEventArgs e)
		{
			base.LockdownServiceAccess();
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeServiceHost";
			}
		}

		protected string RelativeInstallPath
		{
			get
			{
				return "bin";
			}
		}

		public bool ForceFailure;
	}
}
