using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class RemoteDomainMap : RemoteDomainCollection
	{
		public RemoteDomainMap(IList<RemoteDomainEntry> remoteDomainEntryList)
		{
			this.map = new DomainMatchMap<RemoteDomainEntry>(remoteDomainEntryList ?? ((IList<RemoteDomainEntry>)RemoteDomainMap.EmptyRemoteDomainEntryList));
		}

		public static RemoteDomainMap Empty
		{
			get
			{
				return RemoteDomainMap.emptyRemoteDomainMap;
			}
		}

		public RemoteDomainEntry Star
		{
			get
			{
				return this.map.Star;
			}
		}

		public RemoteDomainEntry GetDomainEntry(SmtpDomain domain)
		{
			return this.map.GetBestMatch(domain);
		}

		public override RemoteDomain Find(string domainName)
		{
			return this.map.GetBestMatch(domainName);
		}

		public override IEnumerator<RemoteDomain> GetEnumerator()
		{
			return this.map.GetAllDomains<RemoteDomain>();
		}

		private static readonly RemoteDomainEntry[] EmptyRemoteDomainEntryList = new RemoteDomainEntry[0];

		private static readonly RemoteDomainMap emptyRemoteDomainMap = new RemoteDomainMap(null);

		private readonly DomainMatchMap<RemoteDomainEntry> map;
	}
}
