using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal struct CachedProperty
	{
		public CachedProperty(ADPropertyDefinition adProperty, string extendedProperty)
		{
			this.adProperty = adProperty;
			this.extendedProperty = extendedProperty;
		}

		public ADPropertyDefinition ADProperty
		{
			get
			{
				return this.adProperty;
			}
		}

		public string ExtendedProperty
		{
			get
			{
				return this.extendedProperty;
			}
		}

		public void Set(ADRawEntry entry, TransportMailItem mailItem)
		{
			this.Set(entry, mailItem.ExtendedProperties);
		}

		public void Set(ADRawEntry entry, MailRecipient recipient)
		{
			this.Set(entry, recipient.ExtendedProperties);
		}

		private static object ProxyAddressCollectionToStringList(object data)
		{
			List<string> list = new List<string>(((ProxyAddressCollection)data).ToStringArray());
			if (list.Count != 0)
			{
				return list;
			}
			return null;
		}

		private static object MultiValuedPropertyToList<T>(object data)
		{
			List<T> list = new List<T>((MultiValuedProperty<T>)data);
			if (list.Count != 0)
			{
				return list;
			}
			return null;
		}

		private static object UnlimitedToInt32(object data)
		{
			Unlimited<int> unlimited = (Unlimited<int>)data;
			int? num = null;
			if (!unlimited.IsUnlimited)
			{
				num = new int?(unlimited.Value);
			}
			return num;
		}

		private static object UnlimitedToUInt64(object data)
		{
			Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)data;
			ulong? num = null;
			if (!unlimited.IsUnlimited)
			{
				num = new ulong?(unlimited.Value.ToBytes());
			}
			return num;
		}

		private void Set(ADRawEntry entry, IExtendedPropertyCollection properties)
		{
			object obj = entry[this.adProperty];
			if (obj == null)
			{
				return;
			}
			object obj2;
			if (obj.GetType() == typeof(SmtpAddress))
			{
				obj2 = obj.ToString();
			}
			else if (obj.GetType() == typeof(DeliveryReportsReceiver) || obj.GetType() == typeof(ExternalOofOptions) || obj.GetType() == typeof(TransportModerationNotificationFlags))
			{
				obj2 = (int)obj;
			}
			else if (obj.GetType() == typeof(Unlimited<int>))
			{
				obj2 = CachedProperty.UnlimitedToInt32(obj);
			}
			else if (obj.GetType() == typeof(Unlimited<ByteQuantifiedSize>))
			{
				obj2 = CachedProperty.UnlimitedToUInt64(obj);
			}
			else if (obj.GetType() == typeof(ProxyAddressCollection))
			{
				obj2 = CachedProperty.ProxyAddressCollectionToStringList(obj);
			}
			else if (obj.GetType() == typeof(MultiValuedProperty<string>))
			{
				obj2 = CachedProperty.MultiValuedPropertyToList<string>(obj);
			}
			else if (typeof(MultiValuedProperty<ADObjectId>).IsInstanceOfType(obj))
			{
				obj2 = CachedProperty.MultiValuedPropertyToList<ADObjectId>(obj);
			}
			else if (obj is ProxyAddress)
			{
				obj2 = obj.ToString();
			}
			else
			{
				obj2 = obj;
			}
			if (obj2 != null)
			{
				properties.SetValue<object>(this.extendedProperty, obj2);
			}
		}

		private ADPropertyDefinition adProperty;

		private string extendedProperty;
	}
}
