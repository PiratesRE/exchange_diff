using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class TransportServerIdParameter : RoleServerIdParameter
	{
		public TransportServerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public TransportServerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public TransportServerIdParameter()
		{
		}

		protected TransportServerIdParameter(string identity) : base(identity)
		{
		}

		protected override ServerRole RoleRestriction
		{
			get
			{
				return ServerRole.HubTransport | ServerRole.Edge;
			}
		}

		public new static TransportServerIdParameter Parse(string identity)
		{
			return new TransportServerIdParameter(identity);
		}
	}
}
