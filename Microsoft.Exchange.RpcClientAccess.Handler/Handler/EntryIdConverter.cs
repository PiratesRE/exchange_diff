using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class EntryIdConverter
	{
		public static bool NeedsConversion(PropertyTag propertyTag)
		{
			if (propertyTag.PropertyType == PropertyType.Binary)
			{
				if (propertyTag.PropertyId == PropertyId.ReplyRecipientEntries)
				{
					return true;
				}
				if (Array.IndexOf<PropertyTag>(PropertyTag.OneOffEntryIdPropertyTags, propertyTag) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		public static void ConvertToClient(bool saveAsUnicode, Encoding string8Encoding, ref PropertyValue propertyValue)
		{
			if (propertyValue.PropertyTag.PropertyType == PropertyType.Binary)
			{
				if (propertyValue.PropertyTag.PropertyId == PropertyId.ReplyRecipientEntries)
				{
					AddressEntryList addressEntryList = AddressEntryList.Parse(propertyValue.GetValueAssert<byte[]>(), string8Encoding);
					if (!saveAsUnicode)
					{
						addressEntryList.SetString8(string8Encoding);
					}
					else
					{
						addressEntryList.SetUnicode();
					}
					propertyValue = new PropertyValue(propertyValue.PropertyTag, addressEntryList.Serialize());
					return;
				}
				OneOffEntryId oneOffEntryId = EntryIdConverter.ConvertIfOneOffEntryId(propertyValue.PropertyTag, propertyValue.GetValueAssert<byte[]>(), string8Encoding);
				if (oneOffEntryId != null)
				{
					if (!saveAsUnicode)
					{
						oneOffEntryId.SetString8(string8Encoding);
					}
					else
					{
						oneOffEntryId.SetUnicode();
					}
					propertyValue = new PropertyValue(propertyValue.PropertyTag, oneOffEntryId.Serialize());
				}
			}
		}

		public static void ConvertFromClient(Encoding string8Encoding, ref PropertyValue propertyValue)
		{
			if (propertyValue.PropertyTag.PropertyType == PropertyType.Binary)
			{
				if (propertyValue.PropertyTag.PropertyId == PropertyId.ReplyRecipientEntries)
				{
					AddressEntryList addressEntryList = AddressEntryList.Parse(propertyValue.GetValueAssert<byte[]>(), string8Encoding);
					addressEntryList.SetUnicode();
					propertyValue = new PropertyValue(propertyValue.PropertyTag, addressEntryList.Serialize());
					return;
				}
				OneOffEntryId oneOffEntryId = EntryIdConverter.ConvertIfOneOffEntryId(propertyValue.PropertyTag, propertyValue.GetValueAssert<byte[]>(), string8Encoding);
				if (oneOffEntryId != null)
				{
					oneOffEntryId.SetUnicode();
					propertyValue = new PropertyValue(propertyValue.PropertyTag, oneOffEntryId.Serialize());
				}
			}
		}

		private static OneOffEntryId ConvertIfOneOffEntryId(PropertyTag propertyTag, byte[] entryId, Encoding string8Encoding)
		{
			if (Array.IndexOf<PropertyTag>(PropertyTag.OneOffEntryIdPropertyTags, propertyTag) < 0 || entryId == null || entryId.Length < 24)
			{
				return null;
			}
			using (Reader reader = Reader.CreateBufferReader(entryId))
			{
				string text = null;
				OneOffEntryId result;
				if (OneOffEntryId.TryParse(reader, string8Encoding, out result, (uint)entryId.Length, ref text))
				{
					return result;
				}
			}
			return null;
		}
	}
}
