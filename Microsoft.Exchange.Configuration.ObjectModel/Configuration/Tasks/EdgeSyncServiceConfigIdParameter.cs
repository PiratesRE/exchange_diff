using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class EdgeSyncServiceConfigIdParameter : ADIdParameter
	{
		public EdgeSyncServiceConfigIdParameter()
		{
		}

		public EdgeSyncServiceConfigIdParameter(string identity) : base(identity)
		{
		}

		public EdgeSyncServiceConfigIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public EdgeSyncServiceConfigIdParameter(EdgeSyncServiceConfig edgeSyncServiceConfig) : base(edgeSyncServiceConfig.Id)
		{
		}

		public EdgeSyncServiceConfigIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static EdgeSyncServiceConfigIdParameter Parse(string identity)
		{
			return new EdgeSyncServiceConfigIdParameter(identity);
		}
	}
}
