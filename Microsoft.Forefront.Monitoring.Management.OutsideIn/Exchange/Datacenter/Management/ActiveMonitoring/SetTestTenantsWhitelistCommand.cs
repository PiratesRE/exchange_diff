using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using Microsoft.Datacenter.DataMining.Collection.ActiveMonitoring.MailboxToplogyCollection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Hygiene.Migration;
using Microsoft.Exchange.Management.Powershell.CentralAdmin;

namespace Microsoft.Exchange.Datacenter.Management.ActiveMonitoring
{
	[Cmdlet("Set", "TestTenantsWhitelist")]
	public class SetTestTenantsWhitelistCommand : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public string[] IPAddresses { get; set; }

		[Parameter(Mandatory = false)]
		public string ManagementEndpoint { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Merge { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipValidation { get; set; }

		public new void WriteVerbose(string text)
		{
			base.WriteVerbose(string.Format("[{0}] {1}", DateTime.UtcNow.ToLongTimeString(), text));
		}

		protected override void ProcessRecord()
		{
			List<Exception> list = new List<Exception>();
			IDictionary<string, PSObject> centralAdminForests = IPAddressManagement.GetCentralAdminForests();
			foreach (PSObject singleForest in centralAdminForests.Values)
			{
				try
				{
					IEnumerable<Exception> enumerable = this.Whitelist(singleForest);
					if (enumerable != null && enumerable.Any<Exception>())
					{
						list.AddRange(enumerable);
					}
				}
				catch (Exception item)
				{
					list.Add(item);
				}
			}
			foreach (Exception ex in list)
			{
				base.WriteWarning(ex.ToString());
			}
		}

		private IEnumerable<Exception> Whitelist(PSObject singleForest)
		{
			if (singleForest == null)
			{
				throw new ArgumentException("singleForest");
			}
			List<Exception> list = new List<Exception>();
			IEnumerable<string> forestTypes = TopologyAccess.GetForestTypes(singleForest);
			bool flag = TopologyAccess.IsCapacityForest(forestTypes);
			this.WriteVerbose(string.Format("UpdateIPAllowList.forestTypeCollection <- '{0}'", string.Join(", ", forestTypes)));
			this.WriteVerbose(string.Format("UpdateIPAllowList.isCapacityForest <- {0}", flag));
			if (!flag)
			{
				this.WriteVerbose("Not a Capacity Forest. Skipping.");
				return list;
			}
			string property = ExtensionMethods.GetProperty(singleForest, "Name");
			string forestEnvironment = TopologyAccess.GetForestEnvironment(forestTypes, property);
			bool flag2 = TopologyAccess.IsDedicatedEnvironment(forestEnvironment);
			this.WriteVerbose(string.Format("UpdateIPAllowList.environmentName <- {0}", forestEnvironment));
			this.WriteVerbose(string.Format("UpdateIPAllowList.forestFqdn <- {0}", property));
			this.WriteVerbose(string.Format("UpdateIPAllowList.isDedicatedEnvironment <- {0}", flag2));
			if (flag2)
			{
				this.WriteVerbose(string.Format("{0} is a dedicated environment. Skipping.", forestEnvironment));
				return list;
			}
			string property2 = ExtensionMethods.GetProperty(singleForest, "ActivityState");
			CentralAdminActivityState centralAdminActivityState = (CentralAdminActivityState)Enum.Parse(typeof(CentralAdminActivityState), property2);
			bool flag3 = centralAdminActivityState == 3 || centralAdminActivityState == 1;
			this.WriteVerbose(string.Format("UpdateIPAllowList.isPendingDotBuildUpgrade <- {0} ({1})", flag3, centralAdminActivityState.ToString()));
			if (!flag3)
			{
				this.WriteVerbose(string.Format("{0} is not in PendingDotBuildUpgrade or DotBuildUpgrade state (CentralAdminActivityState = {1}). Skipping.", forestEnvironment, centralAdminActivityState.ToString()));
				return list;
			}
			using (ExchangePowerShell exchangePowerShell = new ExchangePowerShell())
			{
				exchangePowerShell.InitializeExchangeConnection(property, forestEnvironment, null, null);
				Dictionary<string, PSObject> monitoringTenantsByForest = exchangePowerShell.GetMonitoringTenantsByForest(property);
				foreach (TenantProperties tenantProperties in TenantProperties.InitializeFromCollection(monitoringTenantsByForest))
				{
					try
					{
						this.WriteVerbose(string.Format("Processing tenant {0}...", tenantProperties.TenantName));
						this.AddNewIPsToAllowList(exchangePowerShell, tenantProperties, property, forestEnvironment, this.Merge, this.SkipValidation, from e in this.IPAddresses
						select IPAddress.Parse(e));
					}
					catch (Exception ex)
					{
						string arg = exchangePowerShell.GetTenantAdminPassword() ?? string.Empty;
						base.WriteWarning(string.Format("An error occured while processing processing the following tenant. Username = '{0}'; Password = '{1}'", tenantProperties.TenantAdminUpn, arg) + " -- " + ex.ToString());
						list.Add(ex);
					}
				}
			}
			return list;
		}

		private void AddNewIPsToAllowList(ExchangePowerShell exchangePowerShellSession, TenantProperties tenant, string forestFqdn, string environmentName, bool mergeWhitelists, bool skipValidation, IEnumerable<IPAddress> ipsToAddToAllowList)
		{
			if (tenant.Classification == 4)
			{
				this.WriteVerbose(string.Format("AddNewIPsToAllowList - Skipping tenant {0} because Tenant.Classification == BPOSD.", tenant.TenantName));
				return;
			}
			if (tenant.TenantVersion < 15)
			{
				this.WriteVerbose(string.Format("AddNewIPsToAllowList - Skipping tenant {0} because Tenant.TenantVersion < 15.", tenant.TenantName));
				return;
			}
			string text = exchangePowerShellSession.GetTenantAdminPassword() ?? string.Empty;
			RemotePowershellDataConfig remotePowershellDataConfig = new RemotePowershellDataConfig
			{
				ManagementEndpointUri = (string.IsNullOrWhiteSpace(this.ManagementEndpoint) ? "https://ps.outlook.com/PowerShell-LiveID/" : this.ManagementEndpoint),
				BasicAuthUserName = tenant.TenantAdminUpn,
				BasicAuthPassword = text.ConvertToSecureString(),
				SkipCertificateChecks = true,
				UseCertificateAuth = false
			};
			CmdletAuditLog cmdletAuditLog = new CmdletAuditLog("Set-TestTenantsWhitelist", remotePowershellDataConfig);
			ADObjectId adobjectId = new ADObjectId("DN=" + tenant.TenantName);
			this.WriteVerbose(string.Format("AddNewIPsToAllowList - Opening a Remote PowerShell session for {0} (OrgUnitRoot = {1}; Version = {2}; Username = {3}; Password = {4}).", new object[]
			{
				tenant.TenantName,
				adobjectId.ToString(),
				tenant.TenantVersion,
				tenant.TenantAdminUpn,
				text
			}));
			using (RemotePowershellDataProvider remotePowershellDataProvider = new RemotePowershellDataProvider("Set-TestTenantsWhitelist", adobjectId, cmdletAuditLog, remotePowershellDataConfig))
			{
				IEnumerable<HostedConnectionFilterPolicy> enumerable = remotePowershellDataProvider.Find<HostedConnectionFilterPolicy>(null, null, false, null).Cast<HostedConnectionFilterPolicy>();
				this.WriteVerbose(string.Format("AddNewIPsToAllowList - Get-HostedConnectionFilterPolicy <- {0} HostedConnectionFilterPolicies returned.", enumerable.Count<HostedConnectionFilterPolicy>()));
				if (!enumerable.Any<HostedConnectionFilterPolicy>())
				{
					this.WriteVerbose("AddNewIPsToAllowList - Creating a new HostedConnectionFilterPolicy.");
					enumerable = new List<HostedConnectionFilterPolicy>
					{
						new HostedConnectionFilterPolicy
						{
							Name = "Default"
						}
					};
				}
				foreach (HostedConnectionFilterPolicy hostedConnectionFilterPolicy in enumerable)
				{
					bool flag = false;
					if (mergeWhitelists)
					{
						using (IEnumerator<IPAddress> enumerator2 = ipsToAddToAllowList.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								IPAddress ipaddress = enumerator2.Current;
								this.WriteVerbose(string.Format("AddNewIPsToAllowList - Processing (HostedConnectionFilterPolicy.Name = {0}; ipAddress = {1})", hostedConnectionFilterPolicy.Name, ipaddress));
								IPRange newIPRange = IPAddressManagement.GetIPRange(ipaddress);
								if (!hostedConnectionFilterPolicy.IPAllowList.Any((IPRange ipRange) => ipRange.Overlaps(newIPRange)))
								{
									hostedConnectionFilterPolicy.IPAllowList.Add(newIPRange);
									this.WriteVerbose(string.Format("AddNewIPsToAllowList - Adding new IP (HostedConnectionFilterPolicy.Name = {0}; ipAddress = {1})", hostedConnectionFilterPolicy.Name, ipaddress));
									flag = true;
								}
								else
								{
									this.WriteVerbose(string.Format("AddNewIPsToAllowList - IP already present. No update required. (HostedConnectionFilterPolicy.Name = {0}; ipAddress = {1})", hostedConnectionFilterPolicy.Name, ipaddress));
								}
							}
							goto IL_2D3;
						}
						goto IL_271;
					}
					goto IL_271;
					IL_2D3:
					if (flag)
					{
						remotePowershellDataProvider.Save(hostedConnectionFilterPolicy);
						continue;
					}
					continue;
					IL_271:
					this.WriteVerbose(string.Format("AddNewIPsToAllowList - Replacing the existing list of IPs (HostedConnectionFilterPolicy.Name = {0}; ipAddresses = {1})", hostedConnectionFilterPolicy.Name, string.Join<IPAddress>(", ", ipsToAddToAllowList)));
					hostedConnectionFilterPolicy.IPAllowList.Clear();
					hostedConnectionFilterPolicy.IPAllowList.AddRange(from ip in ipsToAddToAllowList
					select IPAddressManagement.GetIPRange(ip));
					flag = true;
					goto IL_2D3;
				}
				if (!skipValidation)
				{
					int num = 0;
					bool flag2 = false;
					while (!flag2 && num++ < 3)
					{
						IEnumerable<HostedConnectionFilterPolicy> enumerable2 = remotePowershellDataProvider.Find<HostedConnectionFilterPolicy>(null, null, false, null).Cast<HostedConnectionFilterPolicy>();
						List<IPAddressManagement.IPWhitelistingErrorDetails> list = new List<IPAddressManagement.IPWhitelistingErrorDetails>();
						foreach (HostedConnectionFilterPolicy hostedConnectionFilterPolicy2 in enumerable2)
						{
							this.WriteVerbose(string.Format("AddNewIPsToAllowList - Checking if IPs have been whitelisted for HostedConnectionFilterPolicy.Name = {0} (Retry = {1}).", hostedConnectionFilterPolicy2.Name, num));
							foreach (IPAddress ipaddress2 in ipsToAddToAllowList)
							{
								IPRange ipToCheck = IPAddressManagement.GetIPRange(ipaddress2);
								if (hostedConnectionFilterPolicy2.IPAllowList.Any((IPRange ipRange) => ipRange.Overlaps(ipToCheck)))
								{
									this.WriteVerbose(string.Format("AddNewIPsToAllowList - IP {0} is present in HostedConnectionFilterPolicy.Name = {1}.", ipaddress2, hostedConnectionFilterPolicy2.Name));
								}
								else
								{
									base.WriteWarning(string.Format("AddNewIPsToAllowList - IP {0} is NOT present in HostedConnectionFilterPolicy.Name = {1}.", ipaddress2, hostedConnectionFilterPolicy2.Name));
									list.Add(new IPAddressManagement.IPWhitelistingErrorDetails(tenant, hostedConnectionFilterPolicy2, ipaddress2));
								}
							}
						}
						flag2 = !list.Any<IPAddressManagement.IPWhitelistingErrorDetails>();
					}
				}
			}
		}
	}
}
