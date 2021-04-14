using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AddressBookEntryId
	{
		private static bool ArrayMatch(byte[] entryid, int offset, byte[] valueToMatch)
		{
			for (int i = 0; i < valueToMatch.Length; i++)
			{
				if (entryid[i + offset] != valueToMatch[i])
				{
					return false;
				}
			}
			return true;
		}

		public static byte[] MakeAddressBookEntryID(IExchangePrincipal exchangePrincipal)
		{
			Util.ThrowOnNullArgument(exchangePrincipal, "exchangePrincipal");
			return AddressBookEntryId.MakeAddressBookEntryID(exchangePrincipal.LegacyDn, false);
		}

		public static byte[] MakeAddressBookEntryID(string legacyDN, bool isDL)
		{
			Eidt eidt = isDL ? Eidt.List : Eidt.User;
			return AddressBookEntryId.MakeAddressBookEntryID(legacyDN, eidt);
		}

		public static byte[] MakeAddressBookEntryID(string legacyDN, Eidt eidt)
		{
			Util.ThrowOnNullOrEmptyArgument(legacyDN, "legacyDN");
			EnumValidator.ThrowIfInvalid<Eidt>(eidt, "eidt");
			byte[] array = new byte[AddressBookEntryId.AddressBookEntryIdSize + legacyDN.Length + 1];
			int num = 0;
			AddressBookEntryId.BinaryHelper.SetDword(array, ref num, 0U);
			Buffer.BlockCopy(AddressBookEntryId.ExchAddrGuid, 0, array, num, AddressBookEntryId.ExchAddrGuid.Length);
			num += AddressBookEntryId.ExchAddrGuid.Length;
			AddressBookEntryId.BinaryHelper.SetDword(array, ref num, AddressBookEntryId.AddressBookEntryIdVersion);
			AddressBookEntryId.BinaryHelper.SetDword(array, ref num, (uint)eidt);
			AddressBookEntryId.BinaryHelper.SetASCIIString(array, ref num, legacyDN);
			return array;
		}

		public static byte[] MakeAddressBookEntryIDFromLocalDirectorySid(SecurityIdentifier sid)
		{
			if (!ExternalUser.IsExternalUserSid(sid))
			{
				throw new InvalidParamException(ServerStrings.InvalidLocalDirectorySecurityIdentifier(sid.ToString()));
			}
			byte[] array = new byte[AddressBookEntryId.AddressBookEntryIdSize + sid.BinaryLength];
			Eidt dw = Eidt.User;
			int num = 0;
			AddressBookEntryId.BinaryHelper.SetDword(array, ref num, 0U);
			Buffer.BlockCopy(AddressBookEntryId.MuidLocalDirectoryUser, 0, array, num, AddressBookEntryId.MuidLocalDirectoryUser.Length);
			num += AddressBookEntryId.ExchAddrGuid.Length;
			AddressBookEntryId.BinaryHelper.SetDword(array, ref num, AddressBookEntryId.AddressBookEntryIdVersion);
			AddressBookEntryId.BinaryHelper.SetDword(array, ref num, (uint)dw);
			sid.GetBinaryForm(array, num);
			return array;
		}

		public static bool IsAddressBookEntryId(byte[] entryId)
		{
			Eidt eidt;
			string text;
			return AddressBookEntryId.IsAddressBookEntryId(entryId, out eidt, out text);
		}

		public static bool IsAddressBookEntryId(byte[] entryId, out Eidt eidt, out string emailAddr)
		{
			eidt = Eidt.User;
			emailAddr = null;
			if (entryId == null)
			{
				return false;
			}
			int num = 0;
			int num2 = entryId.Length;
			bool result = false;
			if (num2 <= AddressBookEntryId.AddressBookEntryIdSize || !AddressBookEntryId.ArrayMatch(entryId, AddressBookEntryId.SkipEntryIdFlagsOffset, AddressBookEntryId.ExchAddrGuid) || entryId[num2 - 1] != 0)
			{
				return result;
			}
			num += AddressBookEntryId.SkipEntryIdFlagsOffset + AddressBookEntryId.ExchAddrGuid.Length + AddressBookEntryId.BinaryHelper.DWordSize;
			try
			{
				eidt = (Eidt)AddressBookEntryId.BinaryHelper.GetDword(entryId, ref num, num2);
				emailAddr = AddressBookEntryId.BinaryHelper.GetStringFromASCII(entryId, ref num, num2);
				result = true;
			}
			catch (ArgumentOutOfRangeException)
			{
			}
			return result;
		}

		public static bool IsLocalDirctoryAddressBookEntryId(byte[] entryId)
		{
			return entryId != null && entryId.Length >= AddressBookEntryId.MinLocalDirectoryAddressBookEntryIdSize && entryId.Length <= AddressBookEntryId.MaxLocalDirectoryAddressBookEntryIdSize && AddressBookEntryId.ArrayMatch(entryId, AddressBookEntryId.SkipEntryIdFlagsOffset, AddressBookEntryId.MuidLocalDirectoryUser);
		}

		public static SecurityIdentifier MakeSidFromLocalDirctoryAddressBookEntryId(byte[] entryId)
		{
			if (!AddressBookEntryId.IsLocalDirctoryAddressBookEntryId(entryId))
			{
				throw new InvalidParamException(new LocalizedString("Invalid local directory address book entry ID."));
			}
			return new SecurityIdentifier(entryId, AddressBookEntryId.AddressBookEntryIdSize);
		}

		public static string MakeLegacyDnFromLocalDirctoryAddressBookEntryId(byte[] entryId)
		{
			SecurityIdentifier securityIdentifier = AddressBookEntryId.MakeSidFromLocalDirctoryAddressBookEntryId(entryId);
			return string.Format("{0}{1}", "LocalUser:", securityIdentifier.ToString());
		}

		internal const string LocalDirectoryUserLegacyDnPrefix = "LocalUser:";

		private static readonly int BytesEntryIdFlags = 4;

		private static readonly int SkipEntryIdFlagsOffset = AddressBookEntryId.BytesEntryIdFlags;

		private static readonly int AddressBookEntryIdSize = 4 * AddressBookEntryId.BinaryHelper.ByteSize + AddressBookEntryId.BinaryHelper.GuidSize + AddressBookEntryId.BinaryHelper.DWordSize + AddressBookEntryId.BinaryHelper.DWordSize;

		private static readonly int MinLocalDirectoryAddressBookEntryIdSize = AddressBookEntryId.AddressBookEntryIdSize + 2 * AddressBookEntryId.BinaryHelper.DWordSize + AddressBookEntryId.BinaryHelper.GuidSize;

		private static readonly int MaxLocalDirectoryAddressBookEntryIdSize = AddressBookEntryId.AddressBookEntryIdSize + 2 * AddressBookEntryId.BinaryHelper.DWordSize + AddressBookEntryId.BinaryHelper.GuidSize + 4;

		private static readonly uint AddressBookEntryIdVersion = 1U;

		private static readonly byte[] ExchAddrGuid = new byte[]
		{
			220,
			167,
			64,
			200,
			192,
			66,
			16,
			26,
			180,
			185,
			8,
			0,
			43,
			47,
			225,
			130
		};

		private static readonly byte[] MuidLocalDirectoryUser = new byte[]
		{
			212,
			186,
			25,
			39,
			241,
			181,
			79,
			27,
			184,
			59,
			20,
			115,
			118,
			55,
			226,
			105
		};

		private static class BinaryHelper
		{
			private static void CheckBounds(int pos, int posMax, int sizeNeeded)
			{
				if (posMax < pos + sizeNeeded)
				{
					throw new ArgumentOutOfRangeException();
				}
			}

			internal static void SetDword(byte[] buff, ref int pos, uint dw)
			{
				if (buff != null)
				{
					AddressBookEntryId.BinaryHelper.CheckBounds(pos, buff.Length, AddressBookEntryId.BinaryHelper.DWordSize);
					ExBitConverter.Write(dw, buff, pos);
				}
				pos += AddressBookEntryId.BinaryHelper.DWordSize;
			}

			internal static void SetASCIIString(byte[] buff, ref int pos, string str)
			{
				if (buff != null)
				{
					AddressBookEntryId.BinaryHelper.CheckBounds(pos, buff.Length, str.Length + 1);
					CTSGlobals.AsciiEncoding.GetBytes(str, 0, str.Length, buff, pos);
					buff[pos + str.Length] = 0;
				}
				pos += str.Length + 1;
			}

			internal static uint GetDword(byte[] buff, ref int pos, int posMax)
			{
				AddressBookEntryId.BinaryHelper.CheckBounds(pos, posMax, AddressBookEntryId.BinaryHelper.DWordSize);
				uint result = (uint)BitConverter.ToInt32(buff, pos);
				pos += AddressBookEntryId.BinaryHelper.DWordSize;
				return result;
			}

			internal static string GetStringFromASCII(byte[] buff, ref int pos, int posMax)
			{
				int num = 0;
				while (pos + num <= posMax && buff[pos + num] != 0)
				{
					num++;
				}
				if (pos + num > posMax)
				{
					throw new ArgumentOutOfRangeException();
				}
				return AddressBookEntryId.BinaryHelper.GetStringFromASCII(buff, ref pos, posMax, num + AddressBookEntryId.BinaryHelper.ByteSize);
			}

			internal static string GetStringFromASCII(byte[] buff, ref int pos, int posMax, int charCount)
			{
				AddressBookEntryId.BinaryHelper.CheckBounds(pos, posMax, charCount);
				string @string = CTSGlobals.AsciiEncoding.GetString(buff, pos, charCount - 1);
				pos += charCount;
				return @string;
			}

			internal static readonly int ByteSize = Marshal.SizeOf(typeof(byte));

			internal static readonly int DWordSize = Marshal.SizeOf(typeof(uint));

			internal static readonly int GuidSize = Marshal.SizeOf(typeof(Guid));
		}
	}
}
