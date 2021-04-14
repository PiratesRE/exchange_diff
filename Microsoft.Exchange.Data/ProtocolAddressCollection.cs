using System;
using System.Collections;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class ProtocolAddressCollection<T> : MultiValuedProperty<T> where T : ProtocolAddress
	{
		public ProtocolAddressCollection()
		{
		}

		public ProtocolAddressCollection(object value) : base(value)
		{
		}

		public ProtocolAddressCollection(ICollection values) : base(values)
		{
		}

		public ProtocolAddressCollection(Hashtable table) : base(table)
		{
		}

		internal ProtocolAddressCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values) : base(readOnly, propertyDefinition, values)
		{
		}

		internal ProtocolAddressCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage) : base(readOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage)
		{
		}

		public string[] ToStringArray()
		{
			string[] array = new string[this.Count];
			for (int i = 0; i < this.Count; i++)
			{
				string[] array2 = array;
				int num = i;
				T t = base[i];
				array2[num] = t.ToString();
			}
			return array;
		}
	}
}
