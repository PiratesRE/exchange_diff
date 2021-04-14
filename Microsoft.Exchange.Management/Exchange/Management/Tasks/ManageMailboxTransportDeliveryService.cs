using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageMailboxTransportDeliveryService : ManageService
	{
		public ManageMailboxTransportDeliveryService()
		{
			base.Account = ServiceAccount.NetworkService;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.MailboxTransportDeliveryServiceDisplayName;
			base.Description = Strings.MailboxTransportDeliveryServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeDelivery.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeDelivery";
			}
		}

		private const string ServiceShortName = "MSExchangeDelivery";

		private const string ServiceBinaryName = "MSExchangeDelivery.exe";

		private const int DeliveryServiceFirstFailureActionDelay = 5000;

		private const int DeliveryServiceSecondFailureActionDelay = 5000;

		private const int DeliveryServiceAllOtherFailuresActionDelay = 5000;

		private const int DeliveryServiceFailureResetPeriod = 0;
	}
}
