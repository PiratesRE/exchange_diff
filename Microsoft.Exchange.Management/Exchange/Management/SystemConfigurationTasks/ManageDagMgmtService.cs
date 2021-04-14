using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class ManageDagMgmtService : ManageService
	{
		protected ManageDagMgmtService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.DagMgmtServiceDisplayName;
			base.Description = Strings.DagMgmtServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeDagMgmt.exe");
			base.ServiceInstallContext = installContext;
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.BinPath, "clusmsg.dll");
			base.CategoryCount = 6;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstaller.AfterInstall += this.AfterInstallEventHandler;
			base.ServicesDependedOn = new List<string>(base.ServicesDependedOn)
			{
				"NetTcpPortSharing"
			}.ToArray();
			base.AddFirewallRule(new MSExchangeDagMgmtWcfServiceFirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeDagMgmt";
			}
		}

		private void AfterInstallEventHandler(object sender, InstallEventArgs e)
		{
			base.LockdownServiceAccess();
		}

		private const string ServiceShortName = "MSExchangeDagMgmt";

		private const string ServiceBinaryName = "MSExchangeDagMgmt.exe";

		private const string EventLogBinaryName = "clusmsg.dll";
	}
}
