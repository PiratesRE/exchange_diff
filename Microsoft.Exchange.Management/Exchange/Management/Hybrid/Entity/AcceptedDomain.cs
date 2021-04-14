using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class AcceptedDomain : IAcceptedDomain
	{
		public AcceptedDomain(AcceptedDomain ad)
		{
			this.DomainNameDomain = ad.DomainName.Domain;
			this.IsCoexistenceDomain = ad.IsCoexistenceDomain;
		}

		public string DomainNameDomain { get; set; }

		public bool IsCoexistenceDomain { get; set; }

		public override string ToString()
		{
			return "{" + string.Format("Domain:'{0}' IsCoexistenceDomain={1}", this.DomainNameDomain, this.IsCoexistenceDomain) + "}";
		}
	}
}
