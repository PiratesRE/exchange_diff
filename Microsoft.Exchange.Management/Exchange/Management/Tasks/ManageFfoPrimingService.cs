using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageFfoPrimingService : ManageService
	{
		protected ManageFfoPrimingService()
		{
			base.Account = ServiceAccount.NetworkService;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.FfoPrimingServiceDisplayName;
			base.Description = Strings.FfoPrimingServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Hygiene.Cache.PrimingService.exe");
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = null;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
		}

		protected override string Name
		{
			get
			{
				return "FfoPrimingService";
			}
		}

		private const string ServiceShortName = "FfoPrimingService";

		private const string ServiceBinaryName = "Microsoft.Exchange.Hygiene.Cache.PrimingService.exe";
	}
}
