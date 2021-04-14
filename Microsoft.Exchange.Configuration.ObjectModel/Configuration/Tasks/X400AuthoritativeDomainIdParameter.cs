using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class X400AuthoritativeDomainIdParameter : ADIdParameter
	{
		public X400AuthoritativeDomainIdParameter()
		{
		}

		public X400AuthoritativeDomainIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public X400AuthoritativeDomainIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected X400AuthoritativeDomainIdParameter(string identity) : base(identity)
		{
		}

		public static X400AuthoritativeDomainIdParameter Parse(string identity)
		{
			return new X400AuthoritativeDomainIdParameter(identity);
		}
	}
}
