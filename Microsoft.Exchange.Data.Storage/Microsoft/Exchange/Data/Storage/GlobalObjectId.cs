using System;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GlobalObjectId : IEquatable<GlobalObjectId>
	{
		public GlobalObjectId()
		{
			this.SetData(Guid.NewGuid().ToByteArray(), true);
		}

		public GlobalObjectId(byte[] globalObjectIdBytes)
		{
			this.SetBytes(globalObjectIdBytes);
		}

		public GlobalObjectId(string uid)
		{
			Util.ThrowOnNullOrEmptyArgument(uid, "uid");
			if (uid.StartsWith("040000008200E00074C5B7101A82E008"))
			{
				byte[] bytes = GlobalObjectId.HexStringToByteArray(uid);
				this.SetBytes(bytes);
				return;
			}
			string text = "vCal-Uid\u0001\0\0\0" + uid;
			this.SetData(GlobalObjectId.StringToByteArray(text, true), false);
		}

		public GlobalObjectId(Item item)
		{
			byte[] valueOrDefault = item.GetValueOrDefault<byte[]>(InternalSchema.GlobalObjectId);
			if (valueOrDefault == null)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidGlobalObjectId);
			}
			this.SetBytes(valueOrDefault);
		}

		public string Uid
		{
			get
			{
				if (this.IsForeignUid())
				{
					int num = 40 + "vCal-Uid\u0001\0\0\0".Length;
					int num2 = this.globalObjectIdBytes.Length;
					int num3 = num2 - num;
					if (this.globalObjectIdBytes[num2 - 1] == 0)
					{
						num3--;
					}
					if (num3 > 0)
					{
						byte[] array = new byte[num3];
						Array.Copy(this.globalObjectIdBytes, num, array, 0, num3);
						return GlobalObjectId.ByteArrayToString(array);
					}
				}
				return GlobalObjectId.ByteArrayToHexString(this.globalObjectIdBytes);
			}
		}

		public ExDateTime Date
		{
			get
			{
				return this.date;
			}
			set
			{
				this.date = value;
				GlobalObjectId.SetDateInBytes(this.globalObjectIdBytes, value);
			}
		}

		public byte[] Bytes
		{
			get
			{
				if (this.globalObjectIdBytes == null)
				{
					return null;
				}
				return (byte[])this.globalObjectIdBytes.Clone();
			}
		}

		public bool IsCleanGlobalObjectId
		{
			get
			{
				return ExDateTime.MinValue.Equals(this.date);
			}
		}

		public byte[] CleanGlobalObjectIdBytes
		{
			get
			{
				byte[] bytes = this.Bytes;
				GlobalObjectId.SetDateInBytes(bytes, ExDateTime.MinValue);
				return bytes;
			}
		}

		public static bool TryParse(string uid, out GlobalObjectId goid)
		{
			goid = null;
			if (!string.IsNullOrEmpty(uid))
			{
				try
				{
					goid = new GlobalObjectId(uid);
				}
				catch (CorruptDataException)
				{
				}
			}
			return goid != null;
		}

		public static bool CompareCleanGlobalObjectIds(byte[] id1, byte[] id2)
		{
			return id1.Length == id2.Length && GlobalObjectId.CompareByteArrays(id1, id2, 0, Math.Min(id1.Length, 16)) && GlobalObjectId.CompareByteArrays(id1, id2, 20, id1.Length);
		}

		public static bool Equals(GlobalObjectId id1, GlobalObjectId id2)
		{
			byte[] array = id1.globalObjectIdBytes;
			byte[] array2 = id2.globalObjectIdBytes;
			return array.Length == array2.Length && GlobalObjectId.CompareByteArrays(array, array2, 0, array.Length);
		}

		public static string ByteArrayToHexString(byte[] array)
		{
			if (array == null)
			{
				return null;
			}
			byte[] array2 = new byte[array.Length * 2];
			int num = 0;
			foreach (byte b in array)
			{
				array2[num++] = GlobalObjectId.NibbleToHex[b >> 4];
				array2[num++] = GlobalObjectId.NibbleToHex[(int)(b & 15)];
			}
			return CTSGlobals.AsciiEncoding.GetString(array2, 0, array2.Length);
		}

		public override string ToString()
		{
			if (this.globalObjectIdBytes == null)
			{
				return string.Empty;
			}
			return GlobalObjectId.ByteArrayToHexString(this.globalObjectIdBytes);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as GlobalObjectId);
		}

		public override int GetHashCode()
		{
			byte[] array = new byte[4];
			for (int i = 0; i < this.globalObjectIdBytes.Length; i++)
			{
				byte[] array2 = array;
				int num = i % 4;
				array2[num] ^= this.globalObjectIdBytes[i];
			}
			int num2 = 0;
			for (int j = 0; j < 4; j++)
			{
				num2 |= (int)array[j] << 8 * j;
			}
			return num2;
		}

		public bool Equals(GlobalObjectId other)
		{
			return other != null && GlobalObjectId.Equals(this, other);
		}

		internal static bool HasInstanceDate(byte[] globalObjectIdBytes)
		{
			return globalObjectIdBytes != null && globalObjectIdBytes.Length >= 20;
		}

		internal static void MakeGlobalObjectIdBytesToClean(byte[] globalObjectIdBytes)
		{
			if (globalObjectIdBytes != null)
			{
				GlobalObjectId.SetDateInBytes(globalObjectIdBytes, ExDateTime.MinValue);
			}
		}

		internal static byte[] HexStringToByteArray(string value)
		{
			if (value.Length % 2 != 0)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidHexString(value));
			}
			int num = value.Length / 2;
			byte[] array = new byte[num];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				byte b = GlobalObjectId.NumFromHex(value[num2++]);
				byte b2 = GlobalObjectId.NumFromHex(value[num2++]);
				array[i] = (byte)((int)b << 4 | (int)b2);
			}
			return array;
		}

		internal static byte[] StringToByteArray(string text)
		{
			return GlobalObjectId.StringToByteArray(text, false);
		}

		internal void SetData(byte[] data, bool setDateTime)
		{
			int num = 40 + data.Length;
			this.globalObjectIdBytes = new byte[num];
			this.nextByteIndex = 0;
			this.Append(GlobalObjectId.SPlusGuid);
			this.SkipBytes(4);
			if (setDateTime)
			{
				long num2 = ExDateTime.UtcNow.ToFileTime();
				byte[] bytes = BitConverter.GetBytes((uint)num2);
				byte[] bytes2 = BitConverter.GetBytes((uint)(num2 >> 32));
				this.Append(bytes);
				this.Append(bytes2);
				this.SkipBytes(8);
			}
			else
			{
				this.SkipBytes(16);
			}
			this.Append(BitConverter.GetBytes(data.Length));
			this.Append(data);
		}

		internal bool IsForeignUid()
		{
			if (this.globalObjectIdBytes == null)
			{
				return false;
			}
			int num = 40 + GlobalObjectId.UidStampArray.Length;
			return this.globalObjectIdBytes.Length >= num && this.globalObjectIdBytes.Skip(40).Take(GlobalObjectId.UidStampArray.Length).SequenceEqual(GlobalObjectId.UidStampArray);
		}

		private static bool CompareByteArrays(byte[] array1, byte[] array2, int startIndex, int endIndex)
		{
			for (int i = startIndex; i < endIndex; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		private static void SetDateInBytes(byte[] bytes, ExDateTime date)
		{
			if (date != ExDateTime.MinValue)
			{
				bytes[16] = (byte)(date.Year >> 8);
				bytes[17] = (byte)(date.Year & 255);
				bytes[18] = (byte)date.Month;
				bytes[19] = (byte)date.Day;
				return;
			}
			bytes[16] = 0;
			bytes[17] = 0;
			bytes[18] = 0;
			bytes[19] = 0;
		}

		private static byte NumFromHex(char ch)
		{
			byte b = (ch < '\u0080') ? GlobalObjectId.HexCharToNum[(int)ch] : byte.MaxValue;
			if (b != 255)
			{
				return b;
			}
			throw new CorruptDataException(ServerStrings.ExInvalidHexCharacter(ch));
		}

		private static byte[] StringToByteArray(string text, bool needNullTerminated)
		{
			byte[] array;
			if (needNullTerminated)
			{
				array = new byte[text.Length + 1];
				array[text.Length] = 0;
			}
			else
			{
				array = new byte[text.Length];
			}
			try
			{
				for (int i = 0; i < text.Length; i++)
				{
					array[i] = Convert.ToByte(text[i]);
				}
			}
			catch (OverflowException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidGlobalObjectId, innerException);
			}
			return array;
		}

		private static string ByteArrayToString(byte[] bytes)
		{
			if (bytes != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					if (bytes[i] == 0)
					{
						throw new CorruptDataException(ServerStrings.ExInvalidGlobalObjectId);
					}
					stringBuilder.Append(Convert.ToChar(bytes[i]));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		private static void CheckGlobalObjectId(byte[] globalObjectIdBytes)
		{
			if (globalObjectIdBytes == null || globalObjectIdBytes.Length < 40)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidGlobalObjectId);
			}
		}

		private void SetBytes(byte[] globalObjectIdBytes)
		{
			GlobalObjectId.CheckGlobalObjectId(globalObjectIdBytes);
			this.globalObjectIdBytes = (byte[])globalObjectIdBytes.Clone();
			int num = (int)this.globalObjectIdBytes[16] << 8 | (int)this.globalObjectIdBytes[17];
			int num2 = (int)this.globalObjectIdBytes[18];
			int num3 = (int)this.globalObjectIdBytes[19];
			if (num == 0 && num2 == 0)
			{
				if (num3 == 0)
				{
					goto IL_6C;
				}
			}
			try
			{
				this.date = new ExDateTime(ExTimeZone.UnspecifiedTimeZone, num, num2, num3);
				return;
			}
			catch (ArgumentOutOfRangeException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidGlobalObjectId, innerException);
			}
			IL_6C:
			this.date = ExDateTime.MinValue;
		}

		private void Append(byte[] bytes)
		{
			Array.Copy(bytes, 0, this.globalObjectIdBytes, this.nextByteIndex, bytes.Length);
			this.nextByteIndex += bytes.Length;
		}

		private void SkipBytes(int bytesToSkip)
		{
			this.nextByteIndex += bytesToSkip;
		}

		private const string SPlusGuidAsString = "040000008200E00074C5B7101A82E008";

		private const string UidStamp = "vCal-Uid\u0001\0\0\0";

		private const int ClsidStart = 0;

		private const int InstanceDateStart = 16;

		private const int NowStart = 20;

		private const int DataLenStart = 36;

		private const int ClsidSize = 16;

		private const int InstanceDateSize = 4;

		private const int NowSize = 16;

		private const int DataLenSize = 4;

		private const int HeaderSize = 40;

		private static readonly byte[] NibbleToHex = CTSGlobals.AsciiEncoding.GetBytes("0123456789ABCDEF");

		private static readonly byte[] HexCharToNum = new byte[]
		{
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			10,
			11,
			12,
			13,
			14,
			15,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			10,
			11,
			12,
			13,
			14,
			15,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue
		};

		private static readonly byte[] UidStampArray = GlobalObjectId.StringToByteArray("vCal-Uid\u0001\0\0\0");

		private static readonly byte[] SPlusGuid = new byte[]
		{
			4,
			0,
			0,
			0,
			130,
			0,
			224,
			0,
			116,
			197,
			183,
			16,
			26,
			130,
			224,
			8
		};

		private byte[] globalObjectIdBytes;

		private ExDateTime date;

		private int nextByteIndex;
	}
}
