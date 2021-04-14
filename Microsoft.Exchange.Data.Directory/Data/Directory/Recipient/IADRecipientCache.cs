using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IADRecipientCache : IEnumerable<KeyValuePair<ProxyAddress, Result<ADRawEntry>>>, IEnumerable
	{
		OrganizationId OrganizationId { get; }

		IRecipientSession ADSession { get; }

		ReadOnlyCollection<ADPropertyDefinition> CachedADProperties { get; }

		ICollection<ProxyAddress> Keys { get; }

		IEnumerable<Result<ADRawEntry>> Values { get; }

		int Count { get; }

		Result<ADRawEntry> FindAndCacheRecipient(ProxyAddress proxyAddress);

		Result<ADRawEntry> FindAndCacheRecipient(ADObjectId objectId);

		IList<Result<ADRawEntry>> FindAndCacheRecipients(IList<ProxyAddress> proxyAddresses);

		void AddCacheEntry(ProxyAddress proxyAddress, Result<ADRawEntry> result);

		bool ContainsKey(ProxyAddress proxyAddress);

		bool CopyEntryFrom(IADRecipientCache cacheToCopyFrom, string smtpAddress);

		bool CopyEntryFrom(IADRecipientCache cacheToCopyFrom, ProxyAddress proxyAddress);

		Result<ADRawEntry> ReadSecurityDescriptor(ProxyAddress proxyAddress);

		void DropSecurityDescriptor(ProxyAddress proxyAddress);

		Result<ADRawEntry> ReloadRecipient(ProxyAddress proxyAddress, IEnumerable<ADPropertyDefinition> extraProperties);

		bool Remove(ProxyAddress proxyAddress);

		bool TryGetValue(ProxyAddress proxyAddress, out Result<ADRawEntry> result);
	}
}
