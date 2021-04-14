using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Data.Transport
{
	public class RoutingOverride
	{
		public RoutingOverride(RoutingDomain routingDomain, DeliveryQueueDomain deliveryQueueDomain) : this(routingDomain, RoutingOverride.EmptyHostList, deliveryQueueDomain)
		{
		}

		public RoutingOverride(RoutingDomain routingDomain, RoutingHost alternateDeliveryRoutingHost, DeliveryQueueDomain deliveryQueueDomain)
		{
			if (routingDomain == RoutingDomain.Empty)
			{
				throw new ArgumentException(string.Format("The provided domain '{0}' is RoutingDomain.Empty which isn't valid for RoutingOverride", routingDomain));
			}
			if (alternateDeliveryRoutingHost == null && deliveryQueueDomain == DeliveryQueueDomain.UseAlternateDeliveryRoutingHosts)
			{
				throw new ArgumentException(string.Format("The provided delivery queue domain value UseAlternateDeliveryRoutingHosts is only valid when you specify alternate delivery hosts.", new object[0]));
			}
			if (alternateDeliveryRoutingHost != null && !string.Equals(routingDomain.Type, "smtp", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(string.Format("The routing domain needs to be an SMTP domain if alternate delivery hosts are specified - routingDomain.Type == {0}", routingDomain.Type));
			}
			this.routingDomain = routingDomain;
			this.alternateDeliveryRoutingHosts = alternateDeliveryRoutingHost.ToString();
			this.deliveryQueueDomain = deliveryQueueDomain;
		}

		public RoutingOverride(RoutingDomain routingDomain, List<RoutingHost> alternateDeliveryRoutingHosts, DeliveryQueueDomain deliveryQueueDomain)
		{
			if (routingDomain == RoutingDomain.Empty)
			{
				throw new ArgumentException(string.Format("The provided domain '{0}' is RoutingDomain.Empty which isn't valid for RoutingOverride", routingDomain));
			}
			if ((alternateDeliveryRoutingHosts == null || alternateDeliveryRoutingHosts.Count == 0) && deliveryQueueDomain == DeliveryQueueDomain.UseAlternateDeliveryRoutingHosts)
			{
				throw new ArgumentException(string.Format("The provided delivery queue domain value UseAlternateDeliveryRoutingHosts is only valid when you specify alternate delivery hosts.", new object[0]));
			}
			if (alternateDeliveryRoutingHosts != null && alternateDeliveryRoutingHosts.Count != 0 && !string.Equals(routingDomain.Type, "smtp", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(string.Format("The routing domain needs to be an SMTP domain if alternate delivery hosts are specified - routingDomain.Type == {0}", routingDomain.Type));
			}
			this.routingDomain = routingDomain;
			this.alternateDeliveryRoutingHosts = RoutingHost.ConvertRoutingHostsToString<RoutingHost>(alternateDeliveryRoutingHosts, (RoutingHost host) => host);
			this.deliveryQueueDomain = deliveryQueueDomain;
		}

		public RoutingDomain RoutingDomain
		{
			get
			{
				return this.routingDomain;
			}
		}

		public List<RoutingHost> AlternateDeliveryRoutingHosts
		{
			get
			{
				return RoutingHost.GetRoutingHostsFromString<RoutingHost>(this.AlternateDeliveryRoutingHostsString, (RoutingHost host) => host);
			}
		}

		internal string AlternateDeliveryRoutingHostsString
		{
			get
			{
				return this.alternateDeliveryRoutingHosts;
			}
		}

		public DeliveryQueueDomain DeliveryQueueDomain
		{
			get
			{
				return this.deliveryQueueDomain;
			}
		}

		public override string ToString()
		{
			int num = (int)this.DeliveryQueueDomain;
			return string.Format(CultureInfo.InvariantCulture, "Connector Domain:[{0}]; Alternate Delivery Routing hosts:[{1}]; Delivery Queue Domain:[{2}]", new object[]
			{
				this.RoutingDomain.ToString(),
				this.AlternateDeliveryRoutingHostsString,
				num
			});
		}

		private static readonly List<RoutingHost> EmptyHostList = new List<RoutingHost>();

		private readonly RoutingDomain routingDomain;

		private readonly string alternateDeliveryRoutingHosts;

		private readonly DeliveryQueueDomain deliveryQueueDomain;
	}
}
