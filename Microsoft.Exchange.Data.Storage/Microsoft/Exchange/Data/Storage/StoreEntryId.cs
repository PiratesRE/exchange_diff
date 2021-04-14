using System;
using System.IO;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class StoreEntryId
	{
		public static string TryParseStoreEntryIdMailboxDN(byte[] storeEntryId)
		{
			Util.ThrowOnNullArgument(storeEntryId, "storeEntryId");
			bool flag = false;
			StoreEntryIdInformation storeEntryIdInformation = StoreEntryId.TryParseStoreEntryId(storeEntryId, out flag);
			if (!flag)
			{
				return null;
			}
			return storeEntryIdInformation.MailboxLegacyDN;
		}

		public static byte[] ToProviderStoreEntryId(IExchangePrincipal exchangePrincipal)
		{
			return StoreEntryId.ToProviderStoreEntryId(exchangePrincipal, false);
		}

		public static byte[] ToProviderStoreEntryId(IExchangePrincipal exchangePrincipal, bool isPublicStore)
		{
			Util.ThrowOnNullArgument(exchangePrincipal, "exchangePrincipal");
			if (exchangePrincipal.MailboxInfo.IsRemote)
			{
				throw new ArgumentException("This method should not be used to remote connections.");
			}
			string mailboxLegacyDn = exchangePrincipal.MailboxInfo.GetMailboxLegacyDn(exchangePrincipal.LegacyDn);
			string serverFqdn = exchangePrincipal.MailboxInfo.Location.ServerFqdn;
			int num = serverFqdn.IndexOf(".", StringComparison.Ordinal);
			string serverNetBiosName = (num != -1) ? serverFqdn.Substring(0, num) : serverFqdn;
			return StoreEntryId.ToProviderStoreEntryId(serverNetBiosName, mailboxLegacyDn, isPublicStore);
		}

		public static string TryParseMailboxLegacyDN(byte[] storeEntryId)
		{
			if (storeEntryId == null)
			{
				return null;
			}
			byte[] array = null;
			using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream()))
			{
				binaryWriter.Write(storeEntryId);
				binaryWriter.BaseStream.Position = 0L;
				MemoryStream memoryStream = binaryWriter.BaseStream as MemoryStream;
				byte[] array2 = new byte[8192];
				int num;
				do
				{
					num = memoryStream.Read(array2, 0, array2.Length);
					for (int i = 0; i < num - 1; i++)
					{
						if (array2[i] == 111 && array2[i + 1] == 61)
						{
							int num2 = num - 1 - (i - 1);
							array = new byte[num2];
							Array.Copy(array2, i - 1, array, 0, num2);
							break;
						}
					}
				}
				while (num > 0);
			}
			if (array == null)
			{
				return null;
			}
			return CTSGlobals.AsciiEncoding.GetString(array, 0, array.Length);
		}

		internal static StoreEntryIdInformation TryParseStoreEntryId(byte[] storeEntryId, out bool isValid)
		{
			isValid = false;
			if (storeEntryId == null)
			{
				return null;
			}
			if (storeEntryId.Length < 30)
			{
				return null;
			}
			StoreEntryIdInformation storeEntryIdInformation = null;
			using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream()))
			{
				binaryWriter.Write(storeEntryId);
				using (BinaryReader binaryReader = new BinaryReader(binaryWriter.BaseStream))
				{
					binaryReader.BaseStream.Position = 0L;
					storeEntryIdInformation = new StoreEntryIdInformation();
					byte[] array = binaryReader.ReadBytes(4);
					for (int i = 0; i < 4; i++)
					{
						if (array[i] != 0)
						{
							return null;
						}
					}
					byte[] array2 = binaryReader.ReadBytes(16);
					if (!ArrayComparer<byte>.Comparer.Equals(array2, StoreEntryId.MuidStoreWrap))
					{
						return null;
					}
					storeEntryIdInformation.WrappedStoreGuid = array2;
					byte b = binaryReader.ReadByte();
					if (b != 0)
					{
						return null;
					}
					byte b2 = binaryReader.ReadByte();
					if (b2 != 0)
					{
						return null;
					}
					byte[] array3 = binaryReader.ReadBytes(14);
					string @string = CTSGlobals.AsciiEncoding.GetString(array3, 0, array3.Length);
					if (!@string.Contains("EMSMDB.DLL"))
					{
						return null;
					}
					storeEntryIdInformation.ProviderFileName = @string;
					storeEntryIdInformation.ProviderFileNameBytes = array3;
					byte[] array4 = binaryReader.ReadBytes(4);
					for (int j = 0; j < 4; j++)
					{
						if (array4[j] != 0)
						{
							return null;
						}
					}
					byte[] array5 = binaryReader.ReadBytes(16);
					if (ArrayComparer<byte>.Comparer.Equals(array5, StoreEntryId.PrivateStore))
					{
						storeEntryIdInformation.IsPublic = false;
					}
					else
					{
						if (!ArrayComparer<byte>.Comparer.Equals(array5, StoreEntryId.PublicStore))
						{
							return null;
						}
						storeEntryIdInformation.IsPublic = true;
					}
					storeEntryIdInformation.StoreGuid = array5;
					uint flagsInt = binaryReader.ReadUInt32();
					storeEntryIdInformation.FlagsInt = flagsInt;
					if ((storeEntryIdInformation.IsPublic && (storeEntryIdInformation.FlagsInt & 6U) == 0U) || (!storeEntryIdInformation.IsPublic && (storeEntryIdInformation.FlagsInt & 12U) == 0U))
					{
						return null;
					}
					storeEntryIdInformation.ServerName = StoreEntryId.ParseStringToTerminator(binaryReader);
					if (string.IsNullOrEmpty(storeEntryIdInformation.ServerName))
					{
						return null;
					}
					storeEntryIdInformation.MailboxLegacyDN = StoreEntryId.ParseStringToTerminator(binaryReader);
					if (string.IsNullOrEmpty(storeEntryIdInformation.MailboxLegacyDN))
					{
						return null;
					}
				}
			}
			isValid = true;
			return storeEntryIdInformation;
		}

		public static byte[] WrapStoreId(byte[] idFromExrpc)
		{
			int num = idFromExrpc.Length;
			byte[] array = new byte[16];
			bool flag = false;
			Array.Copy(idFromExrpc, 4, array, 0, 16);
			if (ArrayComparer<byte>.Comparer.Equals(array, StoreEntryId.PrivateStore))
			{
				flag = false;
			}
			else if (ArrayComparer<byte>.Comparer.Equals(array, StoreEntryId.PublicStore))
			{
				flag = true;
			}
			byte[] array2 = new byte[num + 36];
			for (int i = 0; i < num + 36; i++)
			{
				array2[i] = 0;
			}
			Array.Copy(StoreEntryId.MuidStoreWrap, 0, array2, 4, StoreEntryId.MuidStoreWrap.Length);
			Array.Copy(StoreEntryId.DllFileName, 0, array2, 22, StoreEntryId.DllFileName.Length);
			Array.Copy(idFromExrpc, 4, array2, 40, 16);
			Array.Copy(flag ? StoreEntryId.PublicFlag : StoreEntryId.PrivateFlag, 0, array2, 56, 4);
			Array.Copy(idFromExrpc, 24, array2, 60, num - 24);
			return array2;
		}

		private static byte[] ToProviderStoreEntryId(string serverNetBiosName, string mailboxLegacyDN, bool isPublicStore)
		{
			if (string.IsNullOrEmpty(serverNetBiosName))
			{
				throw new ArgumentNullException("serverNetBiosName");
			}
			if (string.IsNullOrEmpty(mailboxLegacyDN))
			{
				throw new ArgumentNullException("mailboxLegacyDN");
			}
			byte[] result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					uint value = 0U;
					binaryWriter.Write(value);
					binaryWriter.Write(StoreEntryId.MuidStoreWrap);
					binaryWriter.Write(0);
					binaryWriter.Write(0);
					binaryWriter.Write(CTSGlobals.AsciiEncoding.GetBytes("EMSMDB.DLL"));
					binaryWriter.Write(0U);
					binaryWriter.Write(value);
					if (isPublicStore)
					{
						binaryWriter.Write(StoreEntryId.PublicStore);
						binaryWriter.Write(6U);
					}
					else
					{
						binaryWriter.Write(StoreEntryId.PrivateStore);
						binaryWriter.Write(12U);
					}
					binaryWriter.Write(CTSGlobals.AsciiEncoding.GetBytes(serverNetBiosName));
					binaryWriter.Write(0);
					binaryWriter.Write(CTSGlobals.AsciiEncoding.GetBytes(mailboxLegacyDN));
					binaryWriter.Write(0);
					long position = memoryStream.Position;
					binaryWriter.Flush();
					memoryStream.Position = 0L;
					memoryStream.SetLength(position);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		private static string ParseStringToTerminator(BinaryReader reader)
		{
			MemoryStream memoryStream = reader.BaseStream as MemoryStream;
			long position = memoryStream.Position;
			byte[] array = new byte[8192];
			int i;
			for (;;)
			{
				int num = memoryStream.Read(array, 0, array.Length);
				for (i = 0; i < num; i++)
				{
					if (array[i] == 0)
					{
						goto Block_1;
					}
				}
				if (num <= 0)
				{
					goto Block_3;
				}
			}
			Block_1:
			int num2 = i;
			byte[] array2 = new byte[num2];
			Array.Copy(array, 0, array2, 0, num2);
			memoryStream.Position = position + (long)i + 1L;
			return CTSGlobals.AsciiEncoding.GetString(array2, 0, array2.Length);
			Block_3:
			return string.Empty;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static StoreEntryId()
		{
			byte[] array = new byte[4];
			array[0] = 12;
			StoreEntryId.PrivateFlag = array;
			byte[] array2 = new byte[4];
			array2[0] = 6;
			StoreEntryId.PublicFlag = array2;
		}

		private const int ByteCountAbFlags = 4;

		private const int ByteCountMapiUid = 16;

		private const int ByteFileName = 14;

		private const string ProviderFileName = "EMSMDB.DLL";

		private const uint Flags2Public = 6U;

		private const uint Flags2Private = 12U;

		private static readonly byte[] PrivateStore = new byte[]
		{
			27,
			85,
			250,
			32,
			170,
			102,
			17,
			205,
			155,
			200,
			0,
			170,
			0,
			47,
			196,
			90
		};

		private static readonly byte[] PublicStore = new byte[]
		{
			28,
			131,
			2,
			16,
			170,
			102,
			17,
			205,
			155,
			200,
			0,
			170,
			0,
			47,
			196,
			90
		};

		private static readonly byte[] MuidStoreWrap = new byte[]
		{
			56,
			161,
			187,
			16,
			5,
			229,
			16,
			26,
			161,
			187,
			8,
			0,
			43,
			42,
			86,
			194
		};

		private static readonly byte[] DllFileName = new byte[]
		{
			69,
			77,
			83,
			77,
			68,
			66,
			46,
			68,
			76,
			76,
			0,
			0,
			0,
			0
		};

		private static readonly byte[] PrivateFlag;

		private static readonly byte[] PublicFlag;
	}
}
