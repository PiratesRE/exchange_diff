using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class AddressInfoSetter
	{
		public static ErrorCode SetEntryId(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			byte[] array = value as byte[];
			if (array != null)
			{
				value = AddressBookEID.EnsureOneOffEntryIdUnicodeEncoding(context, array);
			}
			bag.SetBlobProperty(context, aie[0], value);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.EntryId, value != null);
			return ErrorCode.NoError;
		}

		public static ErrorCode SetSearchKey(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			return ErrorCode.NoError;
		}

		public static ErrorCode SetAddressType(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			bag.SetBlobProperty(context, aie[2], value);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.AddressType, value != null);
			return ErrorCode.NoError;
		}

		public static ErrorCode SetEmailAddress(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			string text = (string)value;
			if (string.IsNullOrEmpty(text))
			{
				value = null;
			}
			else if (string.Compare(text, 0, "/O=", 0, 3, StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = text.ToUpper();
			}
			bag.SetBlobProperty(context, aie[3], value);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.EmailAddress, value != null);
			return ErrorCode.NoError;
		}

		public static ErrorCode SetDisplayName(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			if (string.IsNullOrEmpty((string)value))
			{
				value = null;
			}
			bag.SetBlobProperty(context, aie[4], value);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.DisplayName, value != null);
			return ErrorCode.NoError;
		}

		public static ErrorCode SetSimpleDisplayName(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			if (string.IsNullOrEmpty((string)value))
			{
				value = null;
			}
			bag.SetBlobProperty(context, aie[5], value);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.SimpleDisplayName, value != null);
			return ErrorCode.NoError;
		}

		public static ErrorCode SetFlags(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			object blobPropertyValue = bag.GetBlobPropertyValue(context, aie[6]);
			int num = (blobPropertyValue == null) ? 0 : ((int)blobPropertyValue & 65535);
			int num2 = (value == null) ? 0 : ((int)value & -65536);
			bag.SetBlobProperty(context, aie[6], num2 | num);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.Flags, value != null);
			return ErrorCode.NoError;
		}

		public static ErrorCode SetOriginalAddressType(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			if (string.IsNullOrEmpty((string)value))
			{
				value = null;
			}
			bag.SetBlobProperty(context, aie[7], value);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.OriginalAddressType, value != null);
			return ErrorCode.NoError;
		}

		public static ErrorCode SetOriginalEmailAddress(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			string text = (string)value;
			if (string.IsNullOrEmpty(text))
			{
				value = null;
			}
			else if (string.Compare(text, 0, "/O=", 0, 3, StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = text.ToUpper();
			}
			bag.SetBlobProperty(context, aie[8], value);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.OriginalEmailAddress, value != null);
			return ErrorCode.NoError;
		}

		public static ErrorCode SetSid(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			bag.SetBlobProperty(context, aie[9], value);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.Sid, value != null);
			return ErrorCode.NoError;
		}

		public static ErrorCode SetGuid(Context context, ISimplePropertyBag bag, StorePropTag[] aie, object value)
		{
			bag.SetBlobProperty(context, aie[10], value);
			AddressInfoSetter.SetInternalFlags(context, bag, aie, AddressInfoEntryFlags.Guid, value != null);
			return ErrorCode.NoError;
		}

		public static void Delete(Context context, ISimplePropertyBag bag, StorePropTag[] aie)
		{
			for (int i = 0; i < aie.Length; i++)
			{
				if (aie[i].PropType != PropertyType.Unspecified)
				{
					bag.SetBlobProperty(context, aie[i], null);
					AddressInfoSetter.SetInternalFlags(context, bag, aie, (AddressInfoEntryFlags)i, false);
				}
			}
		}

		public static void CopyAddressInfoTags(Context context, ISimplePropertyBag bag, AddressInfoTags.AddressInfoType sourceType, AddressInfoTags.AddressInfoType destType)
		{
			StorePropTag[] array = AddressInfoTags.AddressInfoTagList[(int)sourceType];
			StorePropTag[] array2 = AddressInfoTags.AddressInfoTagList[(int)destType];
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, array);
			for (int i = 0; i < array.Length; i++)
			{
				if (i != 1)
				{
					if ((internalFlags & 1 << i) != 0)
					{
						bag.SetBlobProperty(context, array2[i], bag.GetBlobPropertyValue(context, array[i]));
					}
					else
					{
						bag.SetBlobProperty(context, array2[i], null);
					}
				}
			}
			bag.SetBlobProperty(context, array2[6], internalFlags);
		}

		private static void SetInternalFlags(Context context, ISimplePropertyBag bag, StorePropTag[] aie, AddressInfoEntryFlags addressInfoEntryFlags, bool set)
		{
			object blobPropertyValue = bag.GetBlobPropertyValue(context, aie[6]);
			int num = (blobPropertyValue == null) ? 0 : ((int)blobPropertyValue);
			if (set)
			{
				num |= (int)addressInfoEntryFlags;
			}
			else
			{
				num &= (int)(~(int)addressInfoEntryFlags);
			}
			bag.SetBlobProperty(context, aie[6], (num == 0) ? null : num);
		}
	}
}
