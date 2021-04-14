using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class DeliveryGroup : RoutingNextHop
	{
		public abstract IEnumerable<RoutingServerInfo> AllServersNoFallback { get; }

		public abstract string Name { get; }

		public abstract RouteInfo PrimaryRoute { get; }

		public virtual bool IsActive
		{
			get
			{
				return true;
			}
		}

		public override string GetNextHopDomain(RoutingContext context)
		{
			return this.Name;
		}

		public abstract IEnumerable<RoutingServerInfo> GetServersForProxyTarget(ProxyRoutingEnumeratorContext context);

		public virtual IEnumerable<RoutingServerInfo> GetServersForShadowTarget(ProxyRoutingEnumeratorContext context, ShadowRoutingConfiguration shadowRoutingConfig)
		{
			throw new NotSupportedException("This should not be called");
		}

		public virtual bool MayContainServersOfVersion(int majorVersion)
		{
			return true;
		}

		public virtual bool MayContainServersOfVersions(IList<int> majorVersions)
		{
			return true;
		}
	}
}
