using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Tracking
{
	internal class BulkRecipientLookupCache
	{
		internal BulkRecipientLookupCache(int capacity)
		{
			this.lookupCache = new Dictionary<string, string>(capacity, StringComparer.OrdinalIgnoreCase);
		}

		private BulkRecipientLookupCache()
		{
		}

		internal int AddressesLookedUp { get; private set; }

		internal IEnumerable<string> Resolve(IEnumerable<string> addresses, IRecipientSession session)
		{
			if (addresses == null)
			{
				return null;
			}
			int num = 0;
			List<ProxyAddress> list = new List<ProxyAddress>();
			foreach (string text in addresses)
			{
				num++;
				if (!this.lookupCache.ContainsKey(text))
				{
					list.Add(ProxyAddress.Parse(text));
					this.lookupCache[text] = null;
				}
			}
			this.AddressesLookedUp = list.Count;
			if (list.Count > 0)
			{
				ProxyAddress[] array = list.ToArray();
				Result<ADRawEntry>[] array2 = session.FindByProxyAddresses(array, BulkRecipientLookupCache.displayNameProperty);
				for (int i = 0; i < array.Length; i++)
				{
					ADRawEntry data = array2[i].Data;
					string addressString = array[i].AddressString;
					string value = null;
					if (data != null)
					{
						value = (data[ADRecipientSchema.DisplayName] as string);
					}
					if (string.IsNullOrEmpty(value))
					{
						ProxyAddress proxyAddress;
						if (SmtpProxyAddress.TryDeencapsulate(array[i].AddressString, out proxyAddress) && !string.IsNullOrEmpty(proxyAddress.AddressString))
						{
							value = proxyAddress.AddressString;
						}
						else
						{
							value = array[i].AddressString;
						}
					}
					this.lookupCache[addressString] = value;
				}
			}
			return from address in addresses
			select this.lookupCache[address];
		}

		private static PropertyDefinition[] displayNameProperty = new PropertyDefinition[]
		{
			ADRecipientSchema.DisplayName
		};

		private Dictionary<string, string> lookupCache;
	}
}
