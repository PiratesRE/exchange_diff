using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class EdgeSyncMservConnectorIdParameter : ADIdParameter
	{
		public EdgeSyncMservConnectorIdParameter()
		{
		}

		public EdgeSyncMservConnectorIdParameter(string identity) : base(identity)
		{
		}

		public EdgeSyncMservConnectorIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public EdgeSyncMservConnectorIdParameter(EdgeSyncMservConnector connector) : base(connector.Id)
		{
		}

		public EdgeSyncMservConnectorIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static EdgeSyncMservConnectorIdParameter Parse(string identity)
		{
			return new EdgeSyncMservConnectorIdParameter(identity);
		}
	}
}
