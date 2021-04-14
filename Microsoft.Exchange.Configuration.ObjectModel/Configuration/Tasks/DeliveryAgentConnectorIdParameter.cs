using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DeliveryAgentConnectorIdParameter : ADIdParameter
	{
		public DeliveryAgentConnectorIdParameter()
		{
		}

		public DeliveryAgentConnectorIdParameter(string identity) : base(identity)
		{
		}

		public DeliveryAgentConnectorIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public DeliveryAgentConnectorIdParameter(DeliveryAgentConnector connector) : base(connector.Id)
		{
		}

		public static DeliveryAgentConnectorIdParameter Parse(string identity)
		{
			return new DeliveryAgentConnectorIdParameter(identity);
		}
	}
}
