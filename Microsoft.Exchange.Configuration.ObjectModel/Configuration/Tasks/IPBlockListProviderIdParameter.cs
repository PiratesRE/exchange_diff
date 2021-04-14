using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class IPBlockListProviderIdParameter : ADIdParameter
	{
		public IPBlockListProviderIdParameter()
		{
		}

		public IPBlockListProviderIdParameter(string identity) : base(identity)
		{
		}

		public IPBlockListProviderIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public IPBlockListProviderIdParameter(IPBlockListProvider provider) : base(provider.Id)
		{
		}

		public IPBlockListProviderIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static IPBlockListProviderIdParameter Parse(string identity)
		{
			return new IPBlockListProviderIdParameter(identity);
		}
	}
}
