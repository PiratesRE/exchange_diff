using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageMailboxReplicationService : ManageService
	{
		public ManageMailboxReplicationService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.MailboxReplicationServiceDisplayName;
			base.Description = Strings.MailboxReplicationServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeMailboxReplication.exe");
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "Microsoft.Exchange.MailboxReplicationService.EventLog.dll");
			base.CategoryCount = 2;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = new List<string>(base.ServicesDependedOn)
			{
				"NetTcpPortSharing"
			}.ToArray();
			base.AddFirewallRule(new MSExchangeMailboxReplicationFirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeMailboxReplication";
			}
		}

		private const string ServiceShortName = "MSExchangeMailboxReplication";

		private const string ServiceBinaryName = "MSExchangeMailboxReplication.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.MailboxReplicationService.EventLog.dll";
	}
}
