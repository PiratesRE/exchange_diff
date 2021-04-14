using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageMSExchangeMGMTService : ManageService
	{
		protected ManageMSExchangeMGMTService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.MSExchangeMGMTDisplayName;
			base.Description = Strings.MSExchangeMGMTDescription;
			string binPath = ConfigurationContext.Setup.BinPath;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(binPath, "exmgmt.exe");
			base.ServiceInstallContext = installContext;
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "exmgmt.exe");
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeMGMT";
			}
		}

		public bool ForceFailure;
	}
}
