using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RemoteDomainIdParameter : ADIdParameter
	{
		public RemoteDomainIdParameter()
		{
		}

		public RemoteDomainIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public RemoteDomainIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected RemoteDomainIdParameter(string identity) : base(identity)
		{
		}

		public static RemoteDomainIdParameter Parse(string identity)
		{
			return new RemoteDomainIdParameter(identity);
		}
	}
}
