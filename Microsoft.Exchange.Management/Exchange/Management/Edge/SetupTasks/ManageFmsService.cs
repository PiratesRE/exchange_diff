using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	public abstract class ManageFmsService : ManageService
	{
		protected ManageFmsService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.FmsServiceDisplayName;
			base.Description = Strings.FmsServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.FipsBinPath, "FMS.exe");
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = null;
		}

		protected override string Name
		{
			get
			{
				return "FMS";
			}
		}

		private const string ServiceShortName = "FMS";

		private const string ServiceBinaryName = "FMS.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.Common.ProcessManagerMsg.dll";
	}
}
