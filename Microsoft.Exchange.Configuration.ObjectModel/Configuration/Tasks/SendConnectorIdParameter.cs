using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class SendConnectorIdParameter : ADIdParameter
	{
		public SendConnectorIdParameter()
		{
		}

		public SendConnectorIdParameter(string identity) : base(identity)
		{
		}

		public SendConnectorIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public SendConnectorIdParameter(SendConnector connector) : base(connector.Id)
		{
		}

		public SendConnectorIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static SendConnectorIdParameter Parse(string identity)
		{
			return new SendConnectorIdParameter(identity);
		}
	}
}
