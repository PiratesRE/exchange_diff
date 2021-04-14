using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class FrontendTransportServerIdParameter : ExchangeTransportServerIdParameter
	{
		public FrontendTransportServerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public FrontendTransportServerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public FrontendTransportServerIdParameter()
		{
		}

		protected FrontendTransportServerIdParameter(string identity) : base(identity)
		{
		}

		public static FrontendTransportServerIdParameter Parse(string identity)
		{
			return new FrontendTransportServerIdParameter(identity);
		}

		public static FrontendTransportServerIdParameter CreateIdentity(FrontendTransportServerIdParameter identityPassedIn)
		{
			return new FrontendTransportServerIdParameter("Frontend")
			{
				identityPassedIn = identityPassedIn
			};
		}
	}
}
