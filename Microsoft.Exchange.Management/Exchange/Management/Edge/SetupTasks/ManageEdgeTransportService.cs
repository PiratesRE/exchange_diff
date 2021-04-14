using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	public abstract class ManageEdgeTransportService : ManageService
	{
		protected ManageEdgeTransportService()
		{
			base.Account = ServiceAccount.NetworkService;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.EdgeTransportServiceDisplayName;
			base.Description = Strings.EdgeTransportServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeTransport.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = null;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeTransport";
			}
		}

		private const string ServiceShortName = "MSExchangeTransport";

		private const string ServiceBinaryName = "MSExchangeTransport.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.Common.ProcessManagerMsg.dll";
	}
}
