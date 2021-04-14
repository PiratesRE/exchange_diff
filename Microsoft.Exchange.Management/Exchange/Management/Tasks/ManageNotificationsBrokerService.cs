using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageNotificationsBrokerService : ManageService
	{
		public ManageNotificationsBrokerService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.NotificationsBrokerServiceDisplayName;
			base.Description = Strings.NotificationsBrokerServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Notifications.Broker.exe");
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
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeNotificationsBroker";
			}
		}

		private const string ServiceShortName = "MSExchangeNotificationsBroker";

		private const string ServiceBinaryName = "Microsoft.Exchange.Notifications.Broker.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.Notifications.Broker.EventLog.dll";
	}
}
