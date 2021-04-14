using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RmsTrustedPublishingDomainIdParameter : ADIdParameter
	{
		public RmsTrustedPublishingDomainIdParameter()
		{
		}

		public RmsTrustedPublishingDomainIdParameter(string identity) : base(identity)
		{
		}

		public RmsTrustedPublishingDomainIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RmsTrustedPublishingDomainIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static RmsTrustedPublishingDomainIdParameter Parse(string identity)
		{
			return new RmsTrustedPublishingDomainIdParameter(identity);
		}
	}
}
