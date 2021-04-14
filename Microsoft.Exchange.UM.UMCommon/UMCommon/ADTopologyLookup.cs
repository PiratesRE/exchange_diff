using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class ADTopologyLookup
	{
		public static ADTopologyLookup CreateLocalResourceForestLookup()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			ITopologyConfigurationSession topologyConfigurationSession = new LatencyStopwatch().Invoke<ITopologyConfigurationSession>("DirectorySessionFactory.CreateTopologyConfigurationSession", () => DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 77, "CreateLocalResourceForestLookup", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\ADTopologyLookup.cs"));
			return new ADTopologyLookup(topologyConfigurationSession);
		}

		public static ADTopologyLookup CreateTenantResourceForestLookup(Guid orgId)
		{
			string resourceForestFqdnByExternalDirectoryOrganizationId = ADAccountPartitionLocator.GetResourceForestFqdnByExternalDirectoryOrganizationId(orgId);
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "ADTopologyLookup: constructor - resource forest fqdn = {0}", new object[]
			{
				resourceForestFqdnByExternalDirectoryOrganizationId
			});
			PartitionId partitionId = PartitionId.LocalForest;
			if (resourceForestFqdnByExternalDirectoryOrganizationId != null)
			{
				partitionId = new PartitionId(resourceForestFqdnByExternalDirectoryOrganizationId);
			}
			ITopologyConfigurationSession topologyConfigurationSession = new LatencyStopwatch().Invoke<ITopologyConfigurationSession>("DirectorySessionFactory.CreateTopologyConfigurationSession", () => DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 101, "CreateTenantResourceForestLookup", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\ADTopologyLookup.cs"));
			return new ADTopologyLookup(topologyConfigurationSession);
		}

		private ADTopologyLookup(ITopologyConfigurationSession session)
		{
			this.session = session;
		}

		public Server GetLocalServer()
		{
			return this.InvokeWithStopwatch<Server>("ITopologyConfigurationSession.FindLocalServer", () => this.session.FindLocalServer());
		}

		public SIPFEServerConfiguration GetLocalCallRouterSettings()
		{
			return SIPFEServerConfiguration.Find();
		}

		public IEnumerable<Server> GetEnabledExchangeServers(VersionEnum version, ServerRole role)
		{
			return this.InvokeWithStopwatch<ADPagedReader<Server>>("ITopologyConfigurationSession.FindPaged<Server>", () => this.session.FindPaged<Server>(null, QueryScope.SubTree, CommonUtil.GetCompatibleServersWithRole(version, role), null, 100));
		}

		public IEnumerable<Server> GetDatabaseAvailabilityGroupServers(VersionEnum version, ServerRole role, ADObjectId dag)
		{
			if (dag == null)
			{
				throw new ArgumentNullException("dag is null");
			}
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				CommonUtil.GetCompatibleServersWithRole(version, role),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.DatabaseAvailabilityGroup, dag)
			});
			return this.InvokeWithStopwatch<ADPagedReader<Server>>("ITopologyConfigurationSession.FindPaged<Server>", () => this.session.FindPaged<Server>(null, QueryScope.SubTree, queryFilter, null, 100));
		}

		public IEnumerable<Server> GetAllUMServers()
		{
			ExAssert.RetailAssert(!this.isDatacenter, "This method is not intended to be used in Datacenter Environments");
			QueryFilter filter = new BitMaskAndFilter(ServerSchema.CurrentServerRole, 16UL);
			return this.InvokeWithStopwatch<ADPagedReader<Server>>("ITopologyConfigurationSession.FindPaged<Server>", () => this.session.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 100));
		}

		public IEnumerable<Server> GetEnabledExchangeServers(VersionEnum version, ServerRole role, ADObjectId siteId)
		{
			if (siteId == null)
			{
				throw new ArgumentNullException("siteId is null");
			}
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				CommonUtil.GetCompatibleServersWithRole(version, role),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, siteId)
			});
			return this.InvokeWithStopwatch<ADPagedReader<Server>>("ITopologyConfigurationSession.FindPaged<Server>", () => this.session.FindPaged<Server>(null, QueryScope.SubTree, queryFilter, null, 100));
		}

		public IEnumerable<Server> GetEnabledUMServersInSite(VersionEnum version, ADObjectId siteId)
		{
			IEnumerable<Server> enabledExchangeServers;
			if (this.isDatacenter)
			{
				enabledExchangeServers = this.GetEnabledExchangeServers(VersionEnum.Compatible, ServerRole.UnifiedMessaging, siteId);
			}
			else
			{
				enabledExchangeServers = this.GetEnabledExchangeServers(VersionEnum.Compatible, ServerRole.UnifiedMessaging);
			}
			return enabledExchangeServers;
		}

		public IEnumerable<Server> GetAllCafeServers()
		{
			ExAssert.RetailAssert(!this.isDatacenter, "This method is not intended to be used in Datacenter Environments");
			QueryFilter filter = new BitMaskAndFilter(ServerSchema.CurrentServerRole, 1UL);
			return this.InvokeWithStopwatch<ADPagedReader<Server>>("ITopologyConfigurationSession.FindPaged<Server>", () => this.session.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0));
		}

		public IEnumerable<Server> GetEnabledUMServersInDialPlan(VersionEnum version, ADObjectId dialPlanId)
		{
			if (dialPlanId == null)
			{
				throw new ArgumentNullException("dialPlanId");
			}
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				CommonUtil.GetCompatibleServersWithRole(version, ServerRole.UnifiedMessaging),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.DialPlans, dialPlanId)
			});
			return this.InvokeWithStopwatch<ADPagedReader<Server>>("ITopologyConfigurationSession.FindPaged<Server>", () => this.session.FindPaged<Server>(null, QueryScope.SubTree, queryFilter, null, 100));
		}

		public IEnumerable<Server> GetEnabledUMServers(VersionEnum version, ADObjectId dialPlanId, ADObjectId siteId, bool siteAffinityPreferred)
		{
			IEnumerable<Server> enumerable = null;
			if (this.isDatacenter)
			{
				enumerable = this.GetEnabledExchangeServers(version, ServerRole.UnifiedMessaging, siteId);
			}
			else
			{
				if (siteAffinityPreferred)
				{
					Server[] array = ((ADGenericPagedReader<Server>)this.GetEnabledUMServersInDialPlan(version, dialPlanId, siteId)).ReadAllPages();
					if (array.Length > 0)
					{
						enumerable = array;
					}
				}
				if (enumerable == null)
				{
					enumerable = this.GetEnabledUMServersInDialPlan(version, dialPlanId);
				}
			}
			return enumerable;
		}

		public Server GetServerFromName(string serverName)
		{
			if (serverName == null)
			{
				throw new ArgumentNullException("serverName");
			}
			Server result = null;
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, serverName),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.Fqdn, serverName)
			});
			Server[] array = this.InvokeWithStopwatch<Server[]>("ITopologyConfigurationSession.Find<Server>", () => this.session.Find<Server>(null, QueryScope.SubTree, filter, null, 0));
			if (array != null && array.Length == 1)
			{
				result = array[0];
			}
			return result;
		}

		public Server GetServerFromId(ADObjectId serverId)
		{
			if (serverId == null)
			{
				throw new ArgumentNullException("serverId");
			}
			return this.InvokeWithStopwatch<Server>("ITopologyConfigurationSession.Read<Server>(", () => this.session.Read<Server>(serverId));
		}

		public UMServer GetUmServerFromId(ADObjectId serverId)
		{
			if (serverId == null)
			{
				throw new ArgumentNullException("serverId");
			}
			Server server = this.InvokeWithStopwatch<Server>("ITopologyConfigurationSession.Read<Server>(", () => this.session.Read<Server>(serverId));
			if (server != null && (server.CurrentServerRole & ServerRole.UnifiedMessaging) == ServerRole.UnifiedMessaging)
			{
				return new UMServer(server);
			}
			return null;
		}

		public int GetLocalPartnerId()
		{
			ADSite adsite = this.InvokeWithStopwatch<ADSite>("ITopologyConfigurationSession.GetLocalSite", () => this.session.GetLocalSite());
			if (adsite == null)
			{
				return -1;
			}
			return adsite.PartnerId;
		}

		private IEnumerable<Server> GetEnabledUMServersInDialPlan(VersionEnum version, ADObjectId dialPlanId, ADObjectId siteId)
		{
			if (dialPlanId == null)
			{
				throw new ArgumentNullException("dialPlanId");
			}
			if (siteId == null)
			{
				throw new ArgumentNullException("siteId");
			}
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				CommonUtil.GetCompatibleServersWithRole(version, ServerRole.UnifiedMessaging),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.DialPlans, dialPlanId),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, siteId)
			});
			return this.InvokeWithStopwatch<ADPagedReader<Server>>("ITopologyConfigurationSession.FindPaged<Server>", () => this.session.FindPaged<Server>(null, QueryScope.SubTree, queryFilter, null, 100));
		}

		protected T InvokeWithStopwatch<T>(string operationName, Func<T> func)
		{
			return this.latencyStopwatch.Invoke<T>(operationName, func);
		}

		private LatencyStopwatch latencyStopwatch = new LatencyStopwatch();

		private readonly ITopologyConfigurationSession session;

		private readonly bool isDatacenter = CommonConstants.DataCenterADPresent;
	}
}
