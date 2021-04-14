using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ADServiceConnectionPointIdParameter : ADIdParameter
	{
		public ADServiceConnectionPointIdParameter()
		{
		}

		public ADServiceConnectionPointIdParameter(string identity) : base(identity)
		{
		}

		public ADServiceConnectionPointIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ADServiceConnectionPointIdParameter(ADServiceConnectionPoint connectionPoint) : base(connectionPoint.Id)
		{
		}

		public ADServiceConnectionPointIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ADServiceConnectionPointIdParameter Parse(string identity)
		{
			return new ADServiceConnectionPointIdParameter(identity);
		}
	}
}
