using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class AddressInfoGetter
	{
		public static object GetEntryId(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 1) != 0)
			{
				object obj = bag.GetBlobPropertyValue(context, aie[0]);
				byte[] array = obj as byte[];
				if (array != null)
				{
					obj = AddressBookEID.EnsureOneOffEntryIdUnicodeEncoding(context, array);
				}
				return obj;
			}
			if (aieAlter != null && internalFlags == 0)
			{
				return AddressInfoGetter.GetEntryId(context, bag, aieAlter, null);
			}
			return null;
		}

		public static object GetSearchKey(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			if (aie[1] == StorePropTag.Invalid)
			{
				return null;
			}
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 4) != 0 && (internalFlags & 8) != 0)
			{
				string addrType = (string)bag.GetBlobPropertyValue(context, aie[2]);
				string emailAddr = (string)bag.GetBlobPropertyValue(context, aie[3]);
				return AddressBookEID.MakeSearchKey(addrType, emailAddr);
			}
			if ((internalFlags & 1) != 0)
			{
				object blobPropertyValue = bag.GetBlobPropertyValue(context, aie[0]);
				if (blobPropertyValue != null)
				{
					string addrType2 = null;
					string emailAddr2 = null;
					string text = null;
					Eidt eidt;
					if (AddressBookEID.IsAddressBookEntryId(context, (byte[])blobPropertyValue, out eidt, out emailAddr2))
					{
						return AddressBookEID.MakeSearchKey("EX", emailAddr2);
					}
					MapiAPIFlags mapiAPIFlags;
					if (AddressBookEID.IsOneOffEntryId(context, (byte[])blobPropertyValue, out mapiAPIFlags, ref addrType2, ref emailAddr2, ref text))
					{
						return AddressBookEID.MakeSearchKey(addrType2, emailAddr2);
					}
				}
			}
			if (aieAlter != null && internalFlags == 0)
			{
				return AddressInfoGetter.GetSearchKey(context, bag, aieAlter, null);
			}
			return null;
		}

		public static object GetAddressType(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 4) != 0)
			{
				return bag.GetBlobPropertyValue(context, aie[2]);
			}
			if ((internalFlags & 1) != 0)
			{
				object blobPropertyValue = bag.GetBlobPropertyValue(context, aie[0]);
				if (blobPropertyValue != null)
				{
					string result = null;
					string text = null;
					string text2 = null;
					Eidt eidt;
					if (AddressBookEID.IsAddressBookEntryId(context, (byte[])blobPropertyValue, out eidt, out text))
					{
						result = "EX";
					}
					else
					{
						MapiAPIFlags mapiAPIFlags;
						AddressBookEID.IsOneOffEntryId(context, (byte[])blobPropertyValue, out mapiAPIFlags, ref result, ref text, ref text2);
					}
					return result;
				}
			}
			else if (aieAlter != null && internalFlags == 0)
			{
				return AddressInfoGetter.GetAddressType(context, bag, aieAlter, null);
			}
			return null;
		}

		public static object GetEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 8) != 0)
			{
				return bag.GetBlobPropertyValue(context, aie[3]);
			}
			if ((internalFlags & 1) != 0)
			{
				object blobPropertyValue = bag.GetBlobPropertyValue(context, aie[0]);
				if (blobPropertyValue != null)
				{
					string text = null;
					string result = null;
					string text2 = null;
					Eidt eidt;
					if (AddressBookEID.IsAddressBookEntryId(context, (byte[])blobPropertyValue, out eidt, out result))
					{
						text = "EX";
					}
					else
					{
						MapiAPIFlags mapiAPIFlags;
						AddressBookEID.IsOneOffEntryId(context, (byte[])blobPropertyValue, out mapiAPIFlags, ref text, ref result, ref text2);
					}
					return result;
				}
			}
			else if (aieAlter != null && internalFlags == 0)
			{
				return AddressInfoGetter.GetEmailAddress(context, bag, aieAlter, null);
			}
			return null;
		}

		public static object GetDisplayName(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 16) != 0)
			{
				return bag.GetBlobPropertyValue(context, aie[4]);
			}
			if ((internalFlags & 32) != 0)
			{
				return bag.GetBlobPropertyValue(context, aie[5]);
			}
			if ((internalFlags & 1) != 0)
			{
				object blobPropertyValue = bag.GetBlobPropertyValue(context, aie[0]);
				if (blobPropertyValue != null)
				{
					string text = null;
					string text2 = null;
					string result = null;
					Eidt eidt;
					MapiAPIFlags mapiAPIFlags;
					if (AddressBookEID.IsAddressBookEntryId(context, (byte[])blobPropertyValue, out eidt, out text2))
					{
						int num = text2.LastIndexOf("/cn=", StringComparison.OrdinalIgnoreCase);
						if (num != -1 && num + 4 < text2.Length)
						{
							result = text2.Substring(num + 4);
						}
					}
					else if (!AddressBookEID.IsOneOffEntryId(context, (byte[])blobPropertyValue, out mapiAPIFlags, ref text, ref text2, ref result))
					{
						result = null;
					}
					return result;
				}
			}
			else
			{
				if ((internalFlags & 8) != 0)
				{
					return bag.GetBlobPropertyValue(context, aie[3]);
				}
				if (aieAlter != null && internalFlags == 0)
				{
					return AddressInfoGetter.GetDisplayName(context, bag, aieAlter, null);
				}
			}
			return null;
		}

		public static object GetSimpleDisplayName(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 32) != 0)
			{
				return bag.GetBlobPropertyValue(context, aie[5]);
			}
			if ((internalFlags & 1) != 0)
			{
				object blobPropertyValue = bag.GetBlobPropertyValue(context, aie[0]);
				if (blobPropertyValue != null)
				{
					string text = null;
					string text2 = null;
					string result = null;
					Eidt eidt;
					MapiAPIFlags mapiAPIFlags;
					if (AddressBookEID.IsAddressBookEntryId(context, (byte[])blobPropertyValue, out eidt, out text2))
					{
						int num = text2.LastIndexOf("/cn=", StringComparison.OrdinalIgnoreCase);
						if (num != -1 && num + 4 < text2.Length)
						{
							result = text2.Substring(num + 4);
						}
					}
					else if (!AddressBookEID.IsOneOffEntryId(context, (byte[])blobPropertyValue, out mapiAPIFlags, ref text, ref text2, ref result))
					{
						result = null;
					}
					return result;
				}
			}
			else
			{
				if ((internalFlags & 8) != 0)
				{
					return bag.GetBlobPropertyValue(context, aie[3]);
				}
				if (aieAlter != null && internalFlags == 0)
				{
					return AddressInfoGetter.GetSimpleDisplayName(context, bag, aieAlter, null);
				}
			}
			return null;
		}

		public static object GetFlags(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 64) != 0)
			{
				return internalFlags & -65536;
			}
			if (aieAlter != null && internalFlags == 0)
			{
				return AddressInfoGetter.GetFlags(context, bag, aieAlter, null);
			}
			return null;
		}

		public static object GetOriginalAddressType(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 128) != 0)
			{
				return bag.GetBlobPropertyValue(context, aie[7]);
			}
			if (aieAlter != null && internalFlags == 0)
			{
				return AddressInfoGetter.GetOriginalAddressType(context, bag, aieAlter, null);
			}
			return null;
		}

		public static object GetOriginalEmailAddress(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 256) != 0)
			{
				return bag.GetBlobPropertyValue(context, aie[8]);
			}
			if (aieAlter != null && internalFlags == 0)
			{
				return AddressInfoGetter.GetOriginalEmailAddress(context, bag, aieAlter, null);
			}
			return null;
		}

		public static object GetSid(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 512) != 0)
			{
				return bag.GetBlobPropertyValue(context, aie[9]);
			}
			if (aieAlter != null && internalFlags == 0)
			{
				return AddressInfoGetter.GetSid(context, bag, aieAlter, null);
			}
			return null;
		}

		public static object GetGuid(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie, StorePropTag[] aieAlter)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			if ((internalFlags & 1024) != 0)
			{
				return bag.GetBlobPropertyValue(context, aie[10]);
			}
			if (aieAlter != null && internalFlags == 0)
			{
				return AddressInfoGetter.GetGuid(context, bag, aieAlter, null);
			}
			return null;
		}

		public static bool IsAddressInfoTagSet(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie)
		{
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, aie);
			return internalFlags != 0;
		}

		public static bool AddressInfoTagsAreProperSubset(Context context, ISimpleReadOnlyPropertyBag bag, AddressInfoTags.AddressInfoType comperand, AddressInfoTags.AddressInfoType compareTo)
		{
			StorePropTag[] array = AddressInfoTags.AddressInfoTagList[(int)comperand];
			StorePropTag[] array2 = AddressInfoTags.AddressInfoTagList[(int)compareTo];
			int internalFlags = AddressInfoGetter.GetInternalFlags(context, bag, array);
			int internalFlags2 = AddressInfoGetter.GetInternalFlags(context, bag, array2);
			if ((internalFlags & internalFlags2) != internalFlags)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (i != 1 && (internalFlags & 1 << i) != 0 && !ValueHelper.ValuesEqual(bag.GetBlobPropertyValue(context, array[i]), bag.GetBlobPropertyValue(context, array2[i])))
				{
					return false;
				}
			}
			return true;
		}

		internal static int GetInternalFlags(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag[] aie)
		{
			object blobPropertyValue = bag.GetBlobPropertyValue(context, aie[6]);
			int num = (blobPropertyValue == null) ? 0 : ((int)blobPropertyValue);
			return num & -3;
		}
	}
}
