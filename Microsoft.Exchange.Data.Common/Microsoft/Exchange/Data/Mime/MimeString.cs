using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.Mime
{
	internal struct MimeString
	{
		public MimeString(byte[] data)
		{
			this = new MimeString(data, 0, data.Length);
		}

		public MimeString(byte[] data, int offset, int count)
		{
			if ((long)data.Length > 268435455L)
			{
				throw new MimeException(Strings.ValueTooLong);
			}
			this.data = data;
			this.offset = offset;
			this.count = (uint)(count | -268435456);
		}

		public MimeString(byte[] data, int offset, int count, uint mask)
		{
			if ((long)data.Length > 268435455L)
			{
				throw new MimeException(Strings.ValueTooLong);
			}
			this.data = data;
			this.offset = offset;
			this.count = (uint)(count | (int)mask);
		}

		public MimeString(string data)
		{
			if ((long)data.Length > 268435455L)
			{
				throw new MimeException(Strings.ValueTooLong);
			}
			this.data = ByteString.StringToBytes(data, true);
			this.offset = 0;
			this.count = (uint)(this.data.Length | -268435456);
		}

		internal MimeString(MimeString original, int offset, int count)
		{
			this.data = original.data;
			this.offset = offset;
			this.count = (uint)(count | -268435456);
		}

		internal MimeString(MimeString original, int offset, int count, uint mask)
		{
			this.data = original.data;
			this.offset = offset;
			this.count = (uint)(count | (int)mask);
		}

		internal MimeString(byte[] data, int offset, uint countAndMask)
		{
			this.data = data;
			this.offset = offset;
			this.count = countAndMask;
		}

		public int Length
		{
			get
			{
				return (int)(this.count & 268435455U);
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		public uint Mask
		{
			get
			{
				return this.count & 4026531840U;
			}
			set
			{
				this.count = ((this.count & 268435455U) | value);
			}
		}

		public byte this[int index]
		{
			get
			{
				return this.data[this.offset + index];
			}
		}

		internal uint LengthAndMask
		{
			get
			{
				return this.count;
			}
			set
			{
				this.count = value;
			}
		}

		public static bool IsPureASCII(string str)
		{
			bool result = true;
			if (!string.IsNullOrEmpty(str))
			{
				foreach (char c in str)
				{
					if (c >= '\u0080')
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		public static bool IsPureASCII(byte[] bytes)
		{
			bool result = true;
			if (bytes != null)
			{
				foreach (byte b in bytes)
				{
					if (b >= 128)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		public static bool IsPureASCII(MimeString str)
		{
			bool result = true;
			for (int i = 0; i < str.Length; i++)
			{
				byte b = str[i];
				if (b >= 128)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public static bool IsPureASCII(MimeStringList str)
		{
			bool result = true;
			for (int i = 0; i < str.Count; i++)
			{
				MimeString str2 = str[i];
				if (!MimeString.IsPureASCII(str2))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public static MimeString CopyData(byte[] data, int offset, int count)
		{
			byte[] newData = new byte[count];
			ByteEncoder.BlockCopy(data, offset, newData, 0, count);
			return new MimeString(newData, 0, count);
		}

		public override string ToString()
		{
			if (this.data != null)
			{
				return ByteString.BytesToString(this.data, this.offset, this.Length, true);
			}
			return string.Empty;
		}

		public void TrimRight(int count)
		{
			this.count -= (uint)count;
		}

		public byte[] GetSz()
		{
			if (this.data == null || (this.offset == 0 && this.Length == this.data.Length))
			{
				return this.data;
			}
			byte[] array = new byte[this.Length];
			this.CopyTo(array, 0);
			return array;
		}

		public unsafe MimeString CopyData()
		{
			if (this.data == null)
			{
				return default(MimeString);
			}
			byte[] array = new byte[this.Length];
			fixed (byte* ptr = this.data, ptr2 = array)
			{
				byte* ptr3 = ptr + this.offset;
				byte* ptr4 = ptr + this.offset + this.Length;
				byte* ptr5 = ptr2;
				while (ptr3 != ptr4)
				{
					*(ptr5++) = *(ptr3++);
				}
			}
			return new MimeString(array, 0, array.Length, this.Mask);
		}

		public int CopyTo(byte[] destination, int destinationIndex)
		{
			Buffer.BlockCopy(this.data, this.offset, destination, destinationIndex, this.Length);
			return this.Length;
		}

		public void WriteTo(Stream stream)
		{
			stream.Write(this.data, this.offset, this.Length);
		}

		internal uint ComputeCrcI()
		{
			return ByteString.ComputeCrcI(this.data, this.offset, this.Length);
		}

		internal bool CompareEqI(string str2)
		{
			return ByteString.EqualsI(str2, this.Data, this.Offset, this.Length, true);
		}

		internal bool CompareEqI(byte[] str2)
		{
			return ByteString.EqualsI(this.Data, this.Offset, this.Length, str2, 0, str2.Length, true);
		}

		internal bool CompareEqI(MimeString str2)
		{
			return ByteString.EqualsI(this.Data, this.Offset, this.Length, str2.Data, str2.Offset, str2.Length, true);
		}

		internal bool HasPrefixEq(byte[] prefix, int start, int count)
		{
			if (count > this.Length)
			{
				return false;
			}
			int num = -1;
			while (++num < count)
			{
				if (this[num] != prefix[start + num])
				{
					return false;
				}
			}
			return true;
		}

		internal bool HasPrefixEqI(byte[] prefix, int start, int count)
		{
			return count <= this.Length && ByteString.EqualsI(this.Data, this.Offset, count, prefix, start, count, true);
		}

		internal byte[] GetData(out int offset, out int count)
		{
			offset = this.offset;
			count = this.Length;
			return this.data;
		}

		internal bool MergeIfAdjacent(MimeString refString)
		{
			if (this.data == refString.data && this.offset + this.Length == refString.offset && this.Mask == refString.Mask)
			{
				this.count += (uint)refString.Length;
				return true;
			}
			return false;
		}

		internal const string HdrReceived = "Received";

		internal const string HdrContentType = "Content-Type";

		internal const string HdrContentDisposition = "Content-Disposition";

		internal const string HrdDKIMSignature = "DKIM-Signature";

		internal const string HdrXConvertedToMime = "X-ConvertedToMime";

		internal const byte CARRIAGERETURN = 13;

		internal const byte LINEFEED = 10;

		internal const uint LINEFEEDMASK = 168430090U;

		internal const uint LengthMask = 268435455U;

		internal const uint MaskAny = 4026531840U;

		internal const bool AllowUTF8Value = true;

		internal static readonly byte[] EmptyByteArray = new byte[0];

		public static readonly MimeString Empty = new MimeString(MimeString.EmptyByteArray);

		internal static readonly byte[] CrLf = ByteString.StringToBytes("\r\n", true);

		internal static readonly byte[] TwoDashes = ByteString.StringToBytes("--", true);

		internal static readonly byte[] TwoDashesCRLF = ByteString.StringToBytes("--\r\n", true);

		internal static readonly byte[] CRLF2Dashes = ByteString.StringToBytes("\r\n--", true);

		internal static readonly byte[] XParameterNamePrefix = ByteString.StringToBytes("x-", true);

		internal static readonly byte[] Colon = ByteString.StringToBytes(":", true);

		internal static readonly byte[] Comma = ByteString.StringToBytes(",", true);

		internal static readonly byte[] Semicolon = ByteString.StringToBytes(";", true);

		internal static readonly byte[] Space = ByteString.StringToBytes(" ", true);

		internal static readonly byte[] LessThan = ByteString.StringToBytes("<", true);

		internal static readonly byte[] GreaterThan = ByteString.StringToBytes(">", true);

		internal static readonly byte[] DoubleQuote = ByteString.StringToBytes("\"", true);

		internal static readonly byte[] Tabulation = ByteString.StringToBytes("\t", true);

		internal static readonly byte[] Backslash = ByteString.StringToBytes("\\", true);

		internal static readonly byte[] Asterisk = ByteString.StringToBytes("*", true);

		internal static readonly byte[] EqualTo = ByteString.StringToBytes("=", true);

		internal static readonly byte[] CommentInvalidDate = ByteString.StringToBytes("(invalid)", true);

		internal static readonly byte[] Base64 = ByteString.StringToBytes("base64", true);

		internal static readonly byte[] QuotedPrintable = ByteString.StringToBytes("quoted-printable", true);

		internal static readonly byte[] XUuencode = ByteString.StringToBytes("x-uuencode", true);

		internal static readonly byte[] Uue = ByteString.StringToBytes("x-uue", true);

		internal static readonly byte[] Uuencode = ByteString.StringToBytes("uuencode", true);

		internal static readonly byte[] MacBinhex = ByteString.StringToBytes("mac-binhex40", true);

		internal static readonly byte[] Binary = ByteString.StringToBytes("binary", true);

		internal static readonly byte[] Encoding8Bit = ByteString.StringToBytes("8bit", true);

		internal static readonly byte[] Encoding7Bit = ByteString.StringToBytes("7bit", true);

		internal static readonly byte[] Version1 = ByteString.StringToBytes("1.0", true);

		internal static readonly byte[] MimeVersion = ByteString.StringToBytes("MIME-Version: 1.0\r\n", true);

		internal static readonly byte[] TextPlain = ByteString.StringToBytes("text/plain", true);

		internal static readonly byte[] ConvertedToMimeUU = new byte[]
		{
			49
		};

		private byte[] data;

		private int offset;

		private uint count;
	}
}
