using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ProxyFallbackDeliveryGroup : ServerCollectionDeliveryGroup
	{
		public ProxyFallbackDeliveryGroup(RoutedServerCollection fallbackServers) : base(fallbackServers)
		{
		}

		public override string Name
		{
			get
			{
				return "ProxyFallbackDeliveryGroup";
			}
		}

		public override DeliveryType DeliveryType
		{
			get
			{
				return DeliveryType.Undefined;
			}
		}

		public override Guid NextHopGuid
		{
			get
			{
				return Guid.Empty;
			}
		}
	}
}
