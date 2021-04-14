using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ForeignConnectorIdParameter : ADIdParameter
	{
		public ForeignConnectorIdParameter()
		{
		}

		public ForeignConnectorIdParameter(string identity) : base(identity)
		{
		}

		public ForeignConnectorIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ForeignConnectorIdParameter(ForeignConnector connector) : base(connector.Id)
		{
		}

		public ForeignConnectorIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ForeignConnectorIdParameter Parse(string identity)
		{
			return new ForeignConnectorIdParameter(identity);
		}
	}
}
