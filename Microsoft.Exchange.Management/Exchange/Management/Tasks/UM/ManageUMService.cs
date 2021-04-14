using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	public abstract class ManageUMService : ManageService
	{
		protected abstract string ServiceExeName { get; }

		protected abstract string ServiceShortName { get; }

		protected abstract string ServiceDisplayName { get; }

		protected abstract string ServiceDescription { get; }

		protected abstract ExchangeFirewallRule FirewallRule { get; }

		protected abstract string RelativeInstallPath { get; }

		public ManageUMService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = this.ServiceDisplayName;
			base.Description = this.ServiceDescription;
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 10000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 30000U;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.InstallPath, this.RelativeInstallPath, this.ServiceExeName);
			string[] servicesDependedOn = new string[]
			{
				"KeyIso",
				"MSExchangeADTopology"
			};
			base.ServicesDependedOn = servicesDependedOn;
			base.ServiceInstallContext = installContext;
			base.AddFirewallRule(this.FirewallRule);
		}

		public bool ForceFailure
		{
			get
			{
				return this.forceFailure;
			}
			set
			{
				this.forceFailure = value;
			}
		}

		protected override string Name
		{
			get
			{
				return this.ServiceShortName;
			}
		}

		protected void ReservePorts(ushort startPort, ushort numberOfPorts)
		{
			TcpListener.CreatePersistentTcpPortReservation(startPort, numberOfPorts);
		}

		private bool forceFailure;
	}
}
