using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AcceptedDomainMap : AcceptedDomainCollection
	{
		public AcceptedDomainMap(IList<AcceptedDomainEntry> acceptedDomainEntryList)
		{
			this.map = new DomainMatchMap<AcceptedDomainEntry>(acceptedDomainEntryList ?? ((IList<AcceptedDomainEntry>)AcceptedDomainMap.EmptyAcceptedDomainEntryList));
		}

		public bool CheckInternal(SmtpDomain domain)
		{
			AcceptedDomainEntry domainEntry = this.GetDomainEntry(domain);
			return domainEntry != null && domainEntry.IsInternal;
		}

		public AcceptedDomainEntry GetDomainEntry(SmtpDomain domain)
		{
			return this.map.GetBestMatch(domain);
		}

		public bool CheckAccepted(SmtpDomain domain)
		{
			return this.GetDomainEntry(domain) != null;
		}

		public bool CheckAuthoritative(SmtpDomain domain)
		{
			AcceptedDomainEntry domainEntry = this.GetDomainEntry(domain);
			return domainEntry != null && domainEntry.IsAuthoritative;
		}

		public override AcceptedDomain Find(string domainName)
		{
			return this.map.GetBestMatch(domainName);
		}

		public override IEnumerator<AcceptedDomain> GetEnumerator()
		{
			return this.map.GetAllDomains<AcceptedDomain>();
		}

		private static readonly AcceptedDomainEntry[] EmptyAcceptedDomainEntryList = new AcceptedDomainEntry[0];

		private readonly DomainMatchMap<AcceptedDomainEntry> map;
	}
}
