using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ProxyRoutingContext
	{
		public ProxyRoutingContext(RoutingTables routingTables, RoutingContextCore contextCore)
		{
			RoutingUtils.ThrowIfNull(routingTables, "routingTables");
			RoutingUtils.ThrowIfNull(contextCore, "contextCore");
			this.contextCore = contextCore;
			this.routingTables = routingTables;
		}

		public RoutingContextCore Core
		{
			get
			{
				return this.contextCore;
			}
		}

		public virtual int MaxRemoteSiteHubCount
		{
			get
			{
				if (!this.routingTables.ServerMap.LocalHubSiteEnabled)
				{
					return this.contextCore.Settings.ProxyRoutingMaxRemoteSiteHubCount;
				}
				return 0;
			}
		}

		public virtual int MaxLocalSiteHubCount
		{
			get
			{
				return this.MaxTotalHubCount;
			}
		}

		public virtual int MaxTotalHubCount
		{
			get
			{
				return this.contextCore.Settings.ProxyRoutingMaxTotalHubCount;
			}
		}

		public RoutingTables RoutingTables
		{
			get
			{
				return this.routingTables;
			}
		}

		public DateTime Timestamp
		{
			get
			{
				return this.routingTables.WhenCreated;
			}
		}

		public bool XSiteRoutingEnabled
		{
			get
			{
				return this.MaxRemoteSiteHubCount > 0;
			}
		}

		public bool VerifySiteRestriction(RoutingServerInfo serverInfo)
		{
			return this.XSiteRoutingEnabled || this.RoutingTables.ServerMap.IsInLocalSite(serverInfo);
		}

		public bool VerifyVersionRestriction(RoutingServerInfo serverInfo)
		{
			return this.RoutingTables.ServerMap.LocalServerVersion == serverInfo.MajorVersion || this.contextCore.ProxyRoutingAllowedTargetVersions.Contains(serverInfo.MajorVersion);
		}

		public bool VerifyRestrictions(RoutingServerInfo serverInfo)
		{
			return this.VerifyVersionRestriction(serverInfo) && this.VerifySiteRestriction(serverInfo);
		}

		public virtual IEnumerable<RoutingServerInfo> GetDeliveryGroupServers(DeliveryGroup deliveryGroup, ProxyRoutingEnumeratorContext enumeratorContext)
		{
			RoutingUtils.ThrowIfNull(deliveryGroup, "deliveryGroup");
			RoutingUtils.ThrowIfNull(enumeratorContext, "enumeratorContext");
			return deliveryGroup.GetServersForProxyTarget(enumeratorContext);
		}

		private RoutingContextCore contextCore;

		private RoutingTables routingTables;
	}
}
