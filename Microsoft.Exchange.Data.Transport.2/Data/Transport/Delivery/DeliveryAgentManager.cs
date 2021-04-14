using System;

namespace Microsoft.Exchange.Data.Transport.Delivery
{
	public abstract class DeliveryAgentManager : AgentManager
	{
		public abstract string SupportedDeliveryProtocol { get; }
	}
}
