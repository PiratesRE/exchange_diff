using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMIPGateway", DefaultParameterSetName = "Identity")]
	public sealed class GetUMIPGateway : GetMultitenancySystemConfigurationObjectTask<UMIPGatewayIdParameter, UMIPGateway>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSimulator
		{
			get
			{
				return this.includeSimulator;
			}
			set
			{
				this.includeSimulator = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			UMIPGateway umipgateway = (UMIPGateway)dataObject;
			this.InitializeForwardingAddress(umipgateway);
			if (this.IncludeSimulator)
			{
				base.WriteResult(dataObject);
			}
			else if (!umipgateway.Simulator)
			{
				base.WriteResult(dataObject);
			}
			TaskLogger.LogExit();
		}

		private void InitializeForwardingAddress(UMIPGateway gw)
		{
			if (gw.GlobalCallRoutingScheme == UMGlobalCallRoutingScheme.GatewayGuid)
			{
				Guid guid = gw.Guid;
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 111, "InitializeForwardingAddress", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\GetUMIPGateway.cs");
				Server server = topologyConfigurationSession.FindLocalServer();
				List<UMServer> compatibleUMRpcServers = Utility.GetCompatibleUMRpcServers(server.ServerSite, null, topologyConfigurationSession);
				string text = string.Empty;
				using (List<UMServer>.Enumerator enumerator = compatibleUMRpcServers.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						UMServer umserver = enumerator.Current;
						text = umserver.UMForwardingAddressTemplate;
					}
				}
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), gw.OrganizationId, null, false);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 139, "InitializeForwardingAddress", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\GetUMIPGateway.cs");
				ExchangeConfigurationUnit exchangeConfigurationUnit = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(gw.ConfigurationUnit);
				if (!string.IsNullOrEmpty(text) && exchangeConfigurationUnit != null && !string.IsNullOrEmpty(exchangeConfigurationUnit.ExternalDirectoryOrganizationId))
				{
					gw.ForwardingAddress = string.Format(CultureInfo.InvariantCulture, text, new object[]
					{
						exchangeConfigurationUnit.ExternalDirectoryOrganizationId
					});
				}
			}
		}

		private SwitchParameter includeSimulator;
	}
}
