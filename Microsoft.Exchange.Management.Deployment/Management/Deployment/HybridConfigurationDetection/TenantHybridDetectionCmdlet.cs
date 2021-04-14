using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.Hybrid;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	internal class TenantHybridDetectionCmdlet : ITenantHybridDetectionCmdlet
	{
		public void Connect(PSCredential psCredential, string targetServer, ILogger logger)
		{
			string targetServer2 = "ps.outlook.com";
			if (!string.IsNullOrEmpty(targetServer))
			{
				targetServer2 = targetServer;
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Update-HybridConfiguration"))
			{
				if (registryKey != null)
				{
					string text = (string)registryKey.GetValue("TenantPowerShellEndpoint");
					if (!string.IsNullOrEmpty(text))
					{
						targetServer2 = text;
					}
				}
			}
			this.remotePowershellSession = new RemotePowershellSession(targetServer2, PowershellConnectionType.Tenant, true, logger);
			this.remotePowershellSession.Connect(psCredential, CultureInfo.CurrentCulture);
		}

		public IEnumerable<OrganizationConfig> GetOrganizationConfig()
		{
			return this.remotePowershellSession.RunOneCommand<OrganizationConfig>("Get-OrganizationConfig", null, false);
		}

		public void Dispose()
		{
			if (this.remotePowershellSession != null)
			{
				this.remotePowershellSession.Dispose();
				this.remotePowershellSession = null;
			}
		}

		private const string TenantPSEndPoint = "ps.outlook.com";

		private const string GetOrganizationConfigCmdlet = "Get-OrganizationConfig";

		private const string RunHybridRegkeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Update-HybridConfiguration";

		private const string TenantPSEndPointOverride = "TenantPowerShellEndpoint";

		private RemotePowershellSession remotePowershellSession;
	}
}
