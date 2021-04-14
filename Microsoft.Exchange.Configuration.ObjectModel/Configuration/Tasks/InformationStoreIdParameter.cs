using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class InformationStoreIdParameter : ADIdParameter
	{
		public InformationStoreIdParameter()
		{
		}

		public InformationStoreIdParameter(string identity) : base(identity)
		{
		}

		public InformationStoreIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public InformationStoreIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static InformationStoreIdParameter Parse(string identity)
		{
			return new InformationStoreIdParameter(identity);
		}
	}
}
