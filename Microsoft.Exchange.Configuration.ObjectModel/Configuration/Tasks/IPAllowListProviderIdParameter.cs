using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class IPAllowListProviderIdParameter : ADIdParameter
	{
		public IPAllowListProviderIdParameter()
		{
		}

		public IPAllowListProviderIdParameter(string identity) : base(identity)
		{
		}

		public IPAllowListProviderIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public IPAllowListProviderIdParameter(IPAllowListProvider provider) : base(provider.Id)
		{
		}

		public IPAllowListProviderIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static IPAllowListProviderIdParameter Parse(string identity)
		{
			return new IPAllowListProviderIdParameter(identity);
		}
	}
}
