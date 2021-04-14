using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Migration;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class RemotePowershellWrapper : IDisposable
	{
		internal RemotePowershellWrapper(string endPoint, string username, SecureString password, string delegatedTenant = null)
		{
			this.tenantName = username.Split(new char[]
			{
				'@'
			})[1];
			this.remoteConfig = new RemotePowershellDataConfig
			{
				ManagementEndpointUri = endPoint,
				UseCertificateAuth = false,
				BasicAuthUserName = username,
				BasicAuthPassword = password,
				SkipCertificateChecks = true
			};
			this.loggerInstance = new CmdletAuditLog("RemotePowershellHelper", this.remoteConfig);
			this.provider = this.GetDataProvider();
		}

		public IEnumerable<PSObject> Execute(PSCommand psCommand)
		{
			return this.provider.Execute(psCommand, null);
		}

		public IEnumerable<PSObject> Execute(string command)
		{
			return this.provider.Execute(command, null);
		}

		public void Dispose()
		{
			if (this.provider != null)
			{
				this.provider.Dispose();
			}
		}

		private IRemotePowershellDataProvider GetDataProvider()
		{
			return new RemotePowershellDataProvider("RemotePowershellHelper", new ADObjectId("DN=" + this.tenantName), this.loggerInstance, this.remoteConfig);
		}

		private const string CallerId = "RemotePowershellHelper";

		private readonly CmdletAuditLog loggerInstance;

		private readonly RemotePowershellDataConfig remoteConfig;

		private readonly string tenantName;

		private readonly IRemotePowershellDataProvider provider;
	}
}
