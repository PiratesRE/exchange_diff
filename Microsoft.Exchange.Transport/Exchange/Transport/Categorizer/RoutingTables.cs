using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingTables
	{
		public RoutingTables(RoutingTopologyBase topologyConfig, RoutingContextCore context, ITenantDagQuota tenantDagQuota, bool forcedReload)
		{
			RoutingUtils.ThrowIfNull(topologyConfig, "topologyConfig");
			RoutingUtils.ThrowIfNull(context, "context");
			this.PopulateTables(topologyConfig, context, tenantDagQuota, forcedReload);
		}

		public RoutingServerInfoMap ServerMap
		{
			get
			{
				return this.serverMap;
			}
		}

		public DatabaseRouteMap DatabaseMap
		{
			get
			{
				return this.databaseMap;
			}
		}

		public ConnectorIndexMap ConnectorMap
		{
			get
			{
				return this.connectorIndexMap;
			}
		}

		public DateTime WhenCreated
		{
			get
			{
				return this.whenCreated;
			}
		}

		public bool Match(RoutingTables other)
		{
			RoutingUtils.ThrowIfNull(other, "other");
			if (object.ReferenceEquals(this, other))
			{
				throw new InvalidOperationException("An instance of RoutingTables should not be compared with itself");
			}
			return RoutingUtils.NullMatch(this.serverMap, other.serverMap) && RoutingUtils.NullMatch(this.databaseMap, other.databaseMap) && RoutingUtils.NullMatch(this.connectorIndexMap, other.connectorIndexMap) && (this.serverMap == null || this.serverMap.QuickMatch(other.serverMap)) && (this.databaseMap == null || this.databaseMap.QuickMatch(other.databaseMap)) && (this.connectorIndexMap == null || this.connectorIndexMap.QuickMatch(other.connectorIndexMap)) && (this.serverMap == null || this.serverMap.FullMatch(other.serverMap)) && (this.databaseMap == null || this.databaseMap.FullMatch(other.databaseMap)) && (this.connectorIndexMap == null || this.connectorIndexMap.FullMatch(other.connectorIndexMap));
		}

		public bool TryGetDiagnosticInfo(bool verbose, DiagnosableParameters parameters, out XElement diagnosticInfo)
		{
			return this.databaseMap.TryGetDiagnosticInfo(verbose, parameters, out diagnosticInfo);
		}

		private void PopulateTables(RoutingTopologyBase topologyConfig, RoutingContextCore contextCore, ITenantDagQuota tenantDagQuota, bool forcedReload)
		{
			this.whenCreated = topologyConfig.WhenCreated;
			if (contextCore.ServerRoutingSupported)
			{
				this.serverMap = new RoutingServerInfoMap(topologyConfig, contextCore);
			}
			RouteCalculatorContext context = new RouteCalculatorContext(contextCore, topologyConfig, this.serverMap);
			if (contextCore.ConnectorRoutingSupported)
			{
				this.PopulateConnectors(context);
			}
			if (contextCore.ServerRoutingSupported)
			{
				this.PopulateDatabaseRoutes(context, tenantDagQuota, forcedReload);
			}
		}

		private void PopulateConnectors(RouteCalculatorContext context)
		{
			this.connectorIndexMap = new ConnectorIndexMap(this.whenCreated);
			foreach (MailGateway mailGateway in context.TopologyConfig.SendConnectors)
			{
				RouteInfo routeInfo;
				IList<AddressSpace> addressSpaces;
				if (ConnectorRouteFactory.TryCalculateConnectorRoute(mailGateway, mailGateway.HomeMtaServerId, context, out routeInfo, out addressSpaces))
				{
					this.connectorIndexMap.AddConnector(new ConnectorRoutingDestination(mailGateway, addressSpaces, routeInfo));
				}
			}
			if (context.Core.ServerRoutingSupported)
			{
				foreach (KeyValuePair<Guid, RouteInfo> keyValuePair in this.serverMap.RoutingGroupRelayMap.RoutesToRGConnectors)
				{
					this.connectorIndexMap.AddNonAddressSpaceConnector(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		private void PopulateDatabaseRoutes(RouteCalculatorContext context, ITenantDagQuota tenantDagQuota, bool forcedReload)
		{
			this.databaseMap = new DatabaseRouteMap(context, tenantDagQuota, forcedReload);
		}

		private DateTime whenCreated;

		private RoutingServerInfoMap serverMap;

		private DatabaseRouteMap databaseMap;

		private ConnectorIndexMap connectorIndexMap;
	}
}
