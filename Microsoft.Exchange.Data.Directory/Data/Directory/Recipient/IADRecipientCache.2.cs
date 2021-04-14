using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IADRecipientCache<TEntry> where TEntry : ADRawEntry, new()
	{
		OrganizationId OrganizationId { get; }

		IRecipientSession ADSession { get; }

		ReadOnlyCollection<ADPropertyDefinition> CachedADProperties { get; }

		ICollection<ProxyAddress> Keys { get; }

		ICollection<ProxyAddress> ClonedKeys { get; }

		ICollection<Result<TEntry>> Values { get; }

		int Count { get; }

		Result<TEntry> FindAndCacheRecipient(ProxyAddress proxyAddress);

		Result<TEntry> FindAndCacheRecipient(ADObjectId objectId);

		IList<Result<TEntry>> FindAndCacheRecipients(IList<ProxyAddress> proxyAddresses);

		void AddCacheEntry(ProxyAddress proxyAddress, Result<TEntry> result);

		bool ContainsKey(ProxyAddress proxyAddress);

		bool CopyEntryFrom(IADRecipientCache<TEntry> cacheToCopyFrom, string smtpAddress);

		bool CopyEntryFrom(IADRecipientCache<TEntry> cacheToCopyFrom, ProxyAddress proxyAddress);

		IEnumerable<TRecipient> ExpandGroup<TRecipient>(IADDistributionList group) where TRecipient : MiniRecipient, new();

		Result<TEntry> ReadSecurityDescriptor(ProxyAddress proxyAddress);

		void DropSecurityDescriptor(ProxyAddress proxyAddress);

		Result<TEntry> ReloadRecipient(ProxyAddress proxyAddress, IEnumerable<ADPropertyDefinition> extraProperties);

		bool Remove(ProxyAddress proxyAddress);

		bool TryGetValue(ProxyAddress proxyAddress, out Result<TEntry> result);
	}
}
