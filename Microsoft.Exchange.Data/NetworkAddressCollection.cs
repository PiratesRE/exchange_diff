using System;
using System.Collections;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class NetworkAddressCollection : ProtocolAddressCollection<NetworkAddress>
	{
		public new static NetworkAddressCollection Empty
		{
			get
			{
				return NetworkAddressCollection.empty;
			}
		}

		public NetworkAddressCollection()
		{
		}

		public NetworkAddressCollection(object value) : base(value)
		{
		}

		public NetworkAddressCollection(ICollection values) : base(values)
		{
		}

		public NetworkAddressCollection(Hashtable table) : base(table)
		{
		}

		internal NetworkAddressCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values) : base(readOnly, propertyDefinition, values)
		{
		}

		internal NetworkAddressCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage) : base(readOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage)
		{
		}

		public new static implicit operator NetworkAddressCollection(object[] array)
		{
			return new NetworkAddressCollection(false, null, array);
		}

		protected override NetworkAddress ConvertInput(object item)
		{
			if (item is string)
			{
				return NetworkAddress.Parse((string)item);
			}
			return base.ConvertInput(item);
		}

		protected override bool TryAddInternal(NetworkAddress item, out Exception error)
		{
			if (null != item && null != this[item.ProtocolType as NetworkProtocol])
			{
				error = new ArgumentException(DataStrings.ExceptionNetworkProtocolDuplicate, "item");
				return false;
			}
			return base.TryAddInternal(item, out error);
		}

		protected override void SetAt(int index, NetworkAddress item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (base[index] == item)
			{
				return;
			}
			if (base[index].ProtocolType == item.ProtocolType)
			{
				base.SetAt(index, item);
				return;
			}
			NetworkAddress a = this[item.ProtocolType as NetworkProtocol];
			if (a == null)
			{
				base.SetAt(index, item);
				return;
			}
			throw new ArgumentException(DataStrings.ExceptionNetworkProtocolDuplicate, "item");
		}

		public NetworkAddress this[NetworkProtocol protocol]
		{
			get
			{
				NetworkAddress result = null;
				foreach (NetworkAddress networkAddress in this)
				{
					if (networkAddress.ProtocolType == protocol)
					{
						result = networkAddress;
						break;
					}
				}
				return result;
			}
		}

		private static NetworkAddressCollection empty = new NetworkAddressCollection(true, null, new NetworkAddress[0]);
	}
}
