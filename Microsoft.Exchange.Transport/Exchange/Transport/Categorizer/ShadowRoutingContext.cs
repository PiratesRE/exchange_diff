using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ShadowRoutingContext : ProxyRoutingContext
	{
		public ShadowRoutingContext(RoutingTables routingTables, RoutingContextCore contextCore, ShadowRoutingConfiguration shadowRoutingConfiguration) : base(routingTables, contextCore)
		{
			this.shadowRoutingConfig = shadowRoutingConfiguration;
		}

		public override int MaxRemoteSiteHubCount
		{
			get
			{
				return this.shadowRoutingConfig.RemoteShadowCount;
			}
		}

		public override int MaxLocalSiteHubCount
		{
			get
			{
				return this.shadowRoutingConfig.LocalShadowCount;
			}
		}

		public override int MaxTotalHubCount
		{
			get
			{
				return this.shadowRoutingConfig.LocalShadowCount + this.shadowRoutingConfig.RemoteShadowCount;
			}
		}

		public bool EvaluateDeliveryGroup(DeliveryGroup deliveryGroup)
		{
			bool flag = false;
			bool flag2 = false;
			if (deliveryGroup == null)
			{
				return false;
			}
			foreach (RoutingServerInfo serverInfo in deliveryGroup.AllServersNoFallback)
			{
				if (base.RoutingTables.ServerMap.IsInLocalSite(serverInfo))
				{
					flag2 = true;
				}
				else
				{
					flag = true;
				}
				if (flag2 && flag)
				{
					break;
				}
			}
			switch (this.shadowRoutingConfig.ShadowMessagePreference)
			{
			case ShadowMessagePreference.PreferRemote:
				return flag || flag2;
			case ShadowMessagePreference.LocalOnly:
				return flag2;
			case ShadowMessagePreference.RemoteOnly:
				return flag;
			default:
				return false;
			}
		}

		public override IEnumerable<RoutingServerInfo> GetDeliveryGroupServers(DeliveryGroup deliveryGroup, ProxyRoutingEnumeratorContext enumeratorContext)
		{
			RoutingUtils.ThrowIfNull(deliveryGroup, "deliveryGroup");
			RoutingUtils.ThrowIfNull(enumeratorContext, "enumeratorContext");
			return deliveryGroup.GetServersForShadowTarget(enumeratorContext, this.shadowRoutingConfig);
		}

		private ShadowRoutingConfiguration shadowRoutingConfig;
	}
}
