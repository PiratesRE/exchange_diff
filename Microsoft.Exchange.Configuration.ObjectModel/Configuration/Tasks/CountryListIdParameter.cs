using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class CountryListIdParameter : ADIdParameter
	{
		public CountryListIdParameter()
		{
		}

		public CountryListIdParameter(string identity) : base(identity)
		{
		}

		public CountryListIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public CountryListIdParameter(CountryList list) : base(list.Id)
		{
		}

		public CountryListIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static CountryListIdParameter Parse(string identity)
		{
			return new CountryListIdParameter(identity);
		}
	}
}
