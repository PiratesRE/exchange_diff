using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UMIPGatewayIdParameter : ADIdParameter
	{
		public UMIPGatewayIdParameter()
		{
		}

		public UMIPGatewayIdParameter(string identity) : base(identity)
		{
		}

		public UMIPGatewayIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public UMIPGatewayIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public static UMIPGatewayIdParameter Parse(string identity)
		{
			return new UMIPGatewayIdParameter(identity);
		}
	}
}
