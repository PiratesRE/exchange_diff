using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class EmptyRecipientCache : IADRecipientCache, IEnumerable<KeyValuePair<ProxyAddress, Result<ADRawEntry>>>, IEnumerable
	{
		public OrganizationId OrganizationId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IRecipientSession ADSession
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ReadOnlyCollection<ADPropertyDefinition> CachedADProperties
		{
			get
			{
				return new ReadOnlyCollection<ADPropertyDefinition>(new List<ADPropertyDefinition>());
			}
		}

		public ICollection<ProxyAddress> Keys
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<Result<ADRawEntry>> Values
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Count
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		IEnumerator<KeyValuePair<ProxyAddress, Result<ADRawEntry>>> IEnumerable<KeyValuePair<ProxyAddress, Result<ADRawEntry>>>.GetEnumerator()
		{
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			yield break;
		}

		public Result<ADRawEntry> FindAndCacheRecipient(ProxyAddress proxyAddress)
		{
			return new Result<ADRawEntry>(null, ProviderError.NotFound);
		}

		public Result<ADRawEntry> FindAndCacheRecipient(ADObjectId objectId)
		{
			return new Result<ADRawEntry>(null, ProviderError.NotFound);
		}

		public IList<Result<ADRawEntry>> FindAndCacheRecipients(IList<ProxyAddress> proxyAddresses)
		{
			Result<ADRawEntry>[] array = new Result<ADRawEntry>[proxyAddresses.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.FindAndCacheRecipient(proxyAddresses[i]);
			}
			return array;
		}

		public void AddCacheEntry(ProxyAddress proxyAddress, Result<ADRawEntry> result)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(ProxyAddress proxyAddress)
		{
			throw new NotImplementedException();
		}

		public bool CopyEntryFrom(IADRecipientCache cacheToCopyFrom, string smtpAddress)
		{
			throw new NotImplementedException();
		}

		public bool CopyEntryFrom(IADRecipientCache cacheToCopyFrom, ProxyAddress proxyAddress)
		{
			throw new NotImplementedException();
		}

		public Result<ADRawEntry> ReadSecurityDescriptor(ProxyAddress proxyAddress)
		{
			throw new NotImplementedException();
		}

		public void DropSecurityDescriptor(ProxyAddress proxyAddress)
		{
			throw new NotImplementedException();
		}

		public Result<ADRawEntry> ReloadRecipient(ProxyAddress proxyAddress, IEnumerable<ADPropertyDefinition> extraProperties)
		{
			throw new NotImplementedException();
		}

		public bool Remove(ProxyAddress proxyAddress)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(ProxyAddress proxyAddress, out Result<ADRawEntry> result)
		{
			throw new NotImplementedException();
		}
	}
}
