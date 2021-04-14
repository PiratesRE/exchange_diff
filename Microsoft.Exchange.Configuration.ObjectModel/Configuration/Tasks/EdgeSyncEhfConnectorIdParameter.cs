using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class EdgeSyncEhfConnectorIdParameter : ADIdParameter
	{
		public EdgeSyncEhfConnectorIdParameter()
		{
		}

		public EdgeSyncEhfConnectorIdParameter(string identity) : base(identity)
		{
		}

		public EdgeSyncEhfConnectorIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public EdgeSyncEhfConnectorIdParameter(EdgeSyncEhfConnector connector) : base(connector.Id)
		{
		}

		public EdgeSyncEhfConnectorIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static EdgeSyncEhfConnectorIdParameter Parse(string identity)
		{
			return new EdgeSyncEhfConnectorIdParameter(identity);
		}
	}
}
