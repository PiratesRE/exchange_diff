using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class InboundConnectorIdParameter : ADIdParameter
	{
		public InboundConnectorIdParameter()
		{
		}

		public InboundConnectorIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public InboundConnectorIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected InboundConnectorIdParameter(string identity) : base(identity)
		{
		}

		public static InboundConnectorIdParameter Parse(string identity)
		{
			return new InboundConnectorIdParameter(identity);
		}
	}
}
