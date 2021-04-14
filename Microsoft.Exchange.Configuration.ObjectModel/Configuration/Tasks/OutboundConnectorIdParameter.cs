using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class OutboundConnectorIdParameter : ADIdParameter
	{
		public OutboundConnectorIdParameter()
		{
		}

		public OutboundConnectorIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public OutboundConnectorIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected OutboundConnectorIdParameter(string identity) : base(identity)
		{
		}

		public static OutboundConnectorIdParameter Parse(string identity)
		{
			return new OutboundConnectorIdParameter(identity);
		}
	}
}
