using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders
{
	public abstract class RemotePowerShellResponder : ResponderWorkItem
	{
		protected RemotePowerShell RemotePowerShell
		{
			get
			{
				if (this.remotePowershell == null)
				{
					this.CreateRunspace();
				}
				return this.remotePowershell;
			}
		}

		public RemotePowerShellResponder()
		{
		}

		protected void CreateRunspace()
		{
			if (base.Definition.Account == null)
			{
				RemotePowerShellResponder.SetActiveMonitoringCertificateSettings(base.Definition);
				base.Result.StateAttribute5 = "No authentication values were defined in RemoteServerRestartResponder. Certification settings have now been set.";
			}
			if (!string.IsNullOrWhiteSpace(base.Definition.AccountPassword))
			{
				this.remotePowershell = RemotePowerShell.CreateRemotePowerShellByCredential(new Uri(base.Definition.Endpoint), base.Definition.Account, base.Definition.AccountPassword, true);
				return;
			}
			if (base.Definition.Endpoint.Contains(";"))
			{
				this.remotePowershell = RemotePowerShell.CreateRemotePowerShellByCertificate(base.Definition.Endpoint.Split(new char[]
				{
					';'
				}), base.Definition.Account, true);
				return;
			}
			this.remotePowershell = RemotePowerShell.CreateRemotePowerShellByCertificate(new Uri(base.Definition.Endpoint), base.Definition.Account, true);
		}

		internal static void SetActiveMonitoringCertificateSettings(ResponderDefinition definition)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RemotePowerShellResponder.ActiveMonitoringRegistryPath, false))
			{
				if (registryKey != null)
				{
					string text;
					if (definition.Account == null && (text = (string)registryKey.GetValue("RPSCertificateSubject", null)) != null)
					{
						definition.Account = text;
					}
					if (definition.Endpoint == null && (text = (string)registryKey.GetValue("RPSEndpoint", null)) != null)
					{
						definition.Endpoint = text;
					}
				}
			}
		}

		private static readonly string ActiveMonitoringRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\";

		private RemotePowerShell remotePowershell;
	}
}
