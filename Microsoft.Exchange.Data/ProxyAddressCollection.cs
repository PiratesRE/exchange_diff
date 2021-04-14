using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class ProxyAddressCollection : ProxyAddressBaseCollection<ProxyAddress>
	{
		public new static ProxyAddressCollection Empty
		{
			get
			{
				return ProxyAddressCollection.empty;
			}
		}

		public ProxyAddressCollection()
		{
		}

		public ProxyAddressCollection(object value) : base(value)
		{
		}

		public ProxyAddressCollection(ICollection values) : base(values)
		{
		}

		public ProxyAddressCollection(Dictionary<string, object> table) : base(table)
		{
		}

		internal ProxyAddressCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values) : base(readOnly, propertyDefinition, values)
		{
		}

		internal ProxyAddressCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage) : base(readOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage)
		{
		}

		public new static implicit operator ProxyAddressCollection(object[] array)
		{
			return new ProxyAddressCollection(false, null, array);
		}

		internal string GetSipUri()
		{
			foreach (ProxyAddress proxyAddress in this)
			{
				if (proxyAddress.ProxyAddressString.StartsWith("sip:", StringComparison.OrdinalIgnoreCase))
				{
					return proxyAddress.ProxyAddressString.ToLowerInvariant();
				}
			}
			return null;
		}

		public ProxyAddressCollection(Hashtable table) : base(table)
		{
		}

		private static ProxyAddressCollection empty = new ProxyAddressCollection(true, null, new object[0]);
	}
}
