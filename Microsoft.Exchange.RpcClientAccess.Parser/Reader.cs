using System;
using System.IO;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal abstract class Reader : BaseObject
	{
		public abstract long Length { get; }

		public abstract long Position { get; set; }

		public static BufferReader CreateBufferReader(byte[] buffer)
		{
			return Reader.CreateBufferReader(new ArraySegment<byte>(buffer, 0, buffer.Length));
		}

		public static BufferReader CreateBufferReader(ArraySegment<byte> arraySegment)
		{
			return new BufferReader(arraySegment);
		}

		public static Reader CreateStreamReader(Stream stream)
		{
			return new StreamReader(stream);
		}

		public byte PeekByte(long offset)
		{
			base.CheckDisposed();
			long position = this.Position;
			this.Position = position + offset;
			byte result = this.ReadByte();
			this.Position = position;
			return result;
		}

		public ushort PeekUInt16(long offset)
		{
			base.CheckDisposed();
			long position = this.Position;
			this.Position = position + offset;
			ushort result = this.ReadUInt16();
			this.Position = position;
			return result;
		}

		public uint PeekUInt32(long offset)
		{
			base.CheckDisposed();
			long position = this.Position;
			this.Position = position + offset;
			uint result = this.ReadUInt32();
			this.Position = position;
			return result;
		}

		public bool ReadBool()
		{
			base.CheckDisposed();
			return this.ReadByte() != 0;
		}

		public byte ReadByte()
		{
			base.CheckDisposed();
			this.CheckCanRead(1);
			return this.InternalReadByte();
		}

		public byte[] ReadBytes(uint count)
		{
			ArraySegment<byte> arraySegment = this.ReadArraySegment(count);
			if (arraySegment.Count == 0)
			{
				return Array<byte>.Empty;
			}
			byte[] array = new byte[arraySegment.Count];
			Array.Copy(arraySegment.Array, arraySegment.Offset, array, 0, arraySegment.Count);
			return array;
		}

		public ArraySegment<byte> ReadArraySegment(uint count)
		{
			base.CheckDisposed();
			if (count == 0U)
			{
				return Array<byte>.EmptySegment;
			}
			this.CheckCanRead((int)count);
			return this.InternalReadArraySegment(count);
		}

		public double ReadDouble()
		{
			base.CheckDisposed();
			this.CheckCanRead(8);
			return this.InternalReadDouble();
		}

		public Guid ReadGuid()
		{
			base.CheckDisposed();
			ArraySegment<byte> arraySegment = this.ReadArraySegment(16U);
			return ExBitConverter.ReadGuid(arraySegment.Array, arraySegment.Offset);
		}

		public byte[] ReadSizeAndByteArray()
		{
			return this.ReadSizeAndByteArray(FieldLength.WordSize);
		}

		public byte[] ReadSizeAndByteArray(FieldLength lengthSize)
		{
			base.CheckDisposed();
			uint count = this.ReadCountOrSize(lengthSize);
			return this.ReadBytes(count);
		}

		public ArraySegment<byte> ReadSizeAndByteArraySegment()
		{
			return this.ReadSizeAndByteArraySegment(FieldLength.WordSize);
		}

		public ArraySegment<byte> ReadSizeAndByteArraySegment(FieldLength lengthSize)
		{
			base.CheckDisposed();
			uint count = this.ReadCountOrSize(lengthSize);
			return this.ReadArraySegment(count);
		}

		public byte[][] ReadCountAndByteArrayList(FieldLength lengthSize)
		{
			base.CheckDisposed();
			uint num = this.ReadCountOrSize(lengthSize);
			byte[][] array = Array<byte[]>.Empty;
			if (num != 0U)
			{
				array = new byte[num][];
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					array[num2] = this.ReadSizeAndByteArray(lengthSize);
					num2++;
				}
			}
			return array;
		}

		public short ReadInt16()
		{
			base.CheckDisposed();
			this.CheckCanRead(2);
			return this.InternalReadInt16();
		}

		public ushort ReadUInt16()
		{
			base.CheckDisposed();
			this.CheckCanRead(2);
			return this.InternalReadUInt16();
		}

		public int ReadInt32()
		{
			base.CheckDisposed();
			this.CheckCanRead(4);
			return this.InternalReadInt32();
		}

		public uint ReadUInt32()
		{
			base.CheckDisposed();
			this.CheckCanRead(4);
			return this.InternalReadUInt32();
		}

		public long ReadInt64()
		{
			base.CheckDisposed();
			this.CheckCanRead(8);
			return this.InternalReadInt64();
		}

		public ulong ReadUInt64()
		{
			base.CheckDisposed();
			this.CheckCanRead(8);
			return this.InternalReadUInt64();
		}

		public float ReadSingle()
		{
			base.CheckDisposed();
			this.CheckCanRead(4);
			return this.InternalReadSingle();
		}

		public string ReadAsciiString(StringFlags flags)
		{
			String8 @string = this.ReadString8(flags);
			@string.ResolveString8Values(Reader.EncodingWithDecoderFallback(CTSGlobals.AsciiEncoding, flags));
			return @string.StringValue;
		}

		public String8 ReadString8(StringFlags flags)
		{
			ArraySegment<byte> encodedBytes = this.ReadStringSegment(flags);
			return new String8(encodedBytes);
		}

		public string ReadString8(Encoding encoding, StringFlags flags)
		{
			String8 @string = this.ReadString8(flags);
			if ((flags & StringFlags.IncludeNull) != StringFlags.IncludeNull)
			{
				throw new NotSupportedException("Reading non-terminated String8 values is not supported.");
			}
			@string.ResolveString8Values(Reader.EncodingWithDecoderFallback(encoding, flags));
			return @string.StringValue;
		}

		public string ReadUnicodeString(StringFlags flags)
		{
			base.CheckDisposed();
			if ((flags & (StringFlags.IncludeNull | StringFlags.Sized | StringFlags.Sized16 | StringFlags.Sized32)) == StringFlags.None)
			{
				throw new ArgumentException("This ReadString only allows Sized and/or IncludeNull strings.");
			}
			if ((flags & (StringFlags.Sized | StringFlags.Sized16 | StringFlags.Sized32)) == StringFlags.None)
			{
				return this.ReadVariableLengthUnicodeString(flags);
			}
			return this.ReadFixedLengthUnicodeString(flags);
		}

		public SecurityIdentifier ReadSecurityIdentifier()
		{
			uint count = (uint)(8 + 4 * this.PeekByte(1L));
			byte[] binaryForm = this.ReadBytes(count);
			SecurityIdentifier result;
			try
			{
				result = new SecurityIdentifier(binaryForm, 0);
			}
			catch (ArgumentException)
			{
				throw new BufferParseException("Invalid SecurityIdentifier");
			}
			return result;
		}

		public PropertyTag ReadPropertyTag()
		{
			return new PropertyTag(this.ReadUInt32());
		}

		public PropertyValue ReadPropertyValue(WireFormatStyle wireFormatStyle)
		{
			PropertyTag propertyTag = this.ReadPropertyTag();
			return this.ReadPropertyValueForTag(propertyTag, wireFormatStyle);
		}

		public PropertyValue? ReadNullablePropertyValue(WireFormatStyle wireFormatStyle)
		{
			if (wireFormatStyle == WireFormatStyle.Nspi && !this.ReadBool())
			{
				return null;
			}
			return new PropertyValue?(this.ReadPropertyValue(wireFormatStyle));
		}

		public PropertyValue ReadPropertyValueForTag(PropertyTag propertyTag, WireFormatStyle wireFormatStyle)
		{
			object obj = this.ReadPropertyValueForType(propertyTag.PropertyType, wireFormatStyle);
			if (obj == null)
			{
				return PropertyValue.NullValue(propertyTag);
			}
			return new PropertyValue(propertyTag, obj);
		}

		internal void CheckBoundary(uint estimateCount, uint elementSize)
		{
			ulong num = (ulong)(this.Length - this.Position);
			if (num / (ulong)elementSize < (ulong)estimateCount)
			{
				this.Position = this.Length;
				string message = string.Format("Buffer not large enough to accomodate sized array based on minimum element size.  Count = {0}", estimateCount);
				throw new BufferParseException(message);
			}
		}

		internal uint ReadCountOrSize(FieldLength countLength)
		{
			uint result;
			switch (countLength)
			{
			case FieldLength.WordSize:
				result = (uint)this.ReadUInt16();
				break;
			case FieldLength.DWordSize:
				result = this.ReadUInt32();
				break;
			default:
				throw new ArgumentException("Unrecognized FieldLength: " + countLength, "lengthSize");
			}
			return result;
		}

		internal uint ReadCountOrSize(WireFormatStyle wireFormatStyle)
		{
			uint result;
			switch (wireFormatStyle)
			{
			case WireFormatStyle.Rop:
				result = (uint)this.ReadUInt16();
				break;
			case WireFormatStyle.Nspi:
				result = this.ReadUInt32();
				break;
			default:
				throw new ArgumentException("Unrecognized wire format style: " + wireFormatStyle, "wireFormatStyle");
			}
			return result;
		}

		protected abstract byte InternalReadByte();

		protected abstract double InternalReadDouble();

		protected abstract short InternalReadInt16();

		protected abstract ushort InternalReadUInt16();

		protected abstract int InternalReadInt32();

		protected abstract uint InternalReadUInt32();

		protected abstract long InternalReadInt64();

		protected abstract ulong InternalReadUInt64();

		protected abstract float InternalReadSingle();

		protected abstract ArraySegment<byte> InternalReadArraySegment(uint count);

		protected abstract ArraySegment<byte> InternalReadArraySegmentForString(int maxCount);

		protected virtual bool NeedsStagingAreaForFixedLengthStrings
		{
			get
			{
				return false;
			}
		}

		protected byte[] StringBuffer
		{
			get
			{
				return this.charBytes;
			}
		}

		private static ushort ValidateAsciiChar(ushort c, StringFlags flags)
		{
			if (c <= 127)
			{
				return c;
			}
			if ((flags & StringFlags.SevenBitAsciiOrFail) == StringFlags.SevenBitAsciiOrFail)
			{
				throw new BufferParseException("Invalid ASCII character");
			}
			return 63;
		}

		private static string ValidateString(StringBuilder sb, StringFlags flags)
		{
			if (sb.Length == 0)
			{
				return string.Empty;
			}
			int num = sb.Length;
			if ((flags & StringFlags.IncludeNull) == StringFlags.IncludeNull)
			{
				if (sb[sb.Length - 1] != '\0')
				{
					throw new BufferParseException("String doesn't contain a null.");
				}
				num--;
			}
			for (int i = 0; i < num; i++)
			{
				if (sb[i] == '\0')
				{
					num = i;
					break;
				}
				if ((flags & StringFlags.SevenBitAscii) == StringFlags.SevenBitAscii)
				{
					sb[i] = (char)Reader.ValidateAsciiChar((ushort)sb[i], flags);
				}
			}
			if (num != 0)
			{
				return sb.ToString(0, num);
			}
			return string.Empty;
		}

		private ArraySegment<byte> ReadStringSegment(StringFlags flags)
		{
			if ((flags & StringFlags.Sized) != StringFlags.None)
			{
				uint count = (uint)this.ReadByte();
				return this.ReadArraySegment(count);
			}
			if ((flags & StringFlags.Sized16) != StringFlags.None)
			{
				uint count2 = (uint)this.ReadUInt16();
				return this.ReadArraySegment(count2);
			}
			uint num = 0U;
			uint num2 = (uint)(this.Length - this.Position);
			while (this.PeekByte((long)((ulong)num)) != 0)
			{
				num += 1U;
				if (num >= num2)
				{
					throw new BufferParseException("End of buffer reached prematurely.");
				}
			}
			num += 1U;
			return this.ReadArraySegment(num);
		}

		private string ReadVariableLengthUnicodeString(StringFlags flags)
		{
			ushort num = this.ReadUInt16();
			if (num == 0)
			{
				return string.Empty;
			}
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder(Reader.MaxUnicodeCharsSize);
			Decoder decoder = Encoding.Unicode.GetDecoder();
			if (this.charBytes == null)
			{
				this.charBytes = new byte[128];
			}
			if (this.charUnicodeBuffer == null)
			{
				this.charUnicodeBuffer = new char[Reader.MaxUnicodeCharsSize];
			}
			if ((flags & StringFlags.SevenBitAscii) == StringFlags.SevenBitAscii)
			{
				num = Reader.ValidateAsciiChar(num, flags);
			}
			num2 += ExBitConverter.Write(num, this.charBytes, num2);
			while ((num = this.ReadUInt16()) != 0)
			{
				if ((flags & StringFlags.SevenBitAscii) == StringFlags.SevenBitAscii)
				{
					num = Reader.ValidateAsciiChar(num, flags);
				}
				if (num2 >= this.charBytes.Length)
				{
					int chars = decoder.GetChars(this.charBytes, 0, num2, this.charUnicodeBuffer, 0, false);
					stringBuilder.Append(this.charUnicodeBuffer, 0, chars);
					num2 = 0;
				}
				num2 += ExBitConverter.Write(num, this.charBytes, num2);
			}
			if (num2 > 0)
			{
				int chars2 = decoder.GetChars(this.charBytes, 0, num2, this.charUnicodeBuffer, 0, true);
				stringBuilder.Append(this.charUnicodeBuffer, 0, chars2);
			}
			return stringBuilder.ToString();
		}

		private string ReadFixedLengthUnicodeString(StringFlags flags)
		{
			uint length;
			if ((flags & StringFlags.Sized) == StringFlags.Sized)
			{
				length = (uint)this.ReadByte();
			}
			else if ((flags & StringFlags.Sized16) == StringFlags.Sized16)
			{
				length = (uint)this.ReadUInt16();
			}
			else
			{
				length = this.ReadUInt32();
			}
			return this.ReadFixedLengthUnicodeString(flags, length);
		}

		private string ReadFixedLengthUnicodeString(StringFlags flags, uint length)
		{
			int num = 2;
			if ((flags & StringFlags.IncludeNull) == StringFlags.IncludeNull && length < (uint)num)
			{
				throw new BufferParseException("Length cannot be less than a single character when IncludeNull is specified.");
			}
			if (length == 0U)
			{
				return string.Empty;
			}
			if (length % 2U != 0U)
			{
				throw new BufferParseException("Unicode string is not even length.");
			}
			Decoder decoder = Encoding.Unicode.GetDecoder();
			if (this.charUnicodeBuffer == null)
			{
				this.charUnicodeBuffer = new char[Reader.MaxUnicodeCharsSize];
			}
			char[] array = this.charUnicodeBuffer;
			if (this.charBytes == null && this.NeedsStagingAreaForFixedLengthStrings)
			{
				this.charBytes = new byte[128];
			}
			StringBuilder stringBuilder = new StringBuilder(checked((int)length));
			uint num2 = 0U;
			for (;;)
			{
				int maxCount = Math.Min((int)(length - num2), 128);
				ArraySegment<byte> arraySegment = this.InternalReadArraySegmentForString(maxCount);
				if (arraySegment.Count == 0)
				{
					break;
				}
				int chars = decoder.GetChars(arraySegment.Array, arraySegment.Offset, arraySegment.Count, array, 0);
				stringBuilder.Append(array, 0, chars);
				num2 += (uint)arraySegment.Count;
				if (num2 >= length)
				{
					goto Block_9;
				}
			}
			throw new BufferParseException("End of buffer reached prematurely.");
			Block_9:
			return Reader.ValidateString(stringBuilder, flags);
		}

		private void CheckCanRead(int size)
		{
			if (this.Position > this.Length - (long)size)
			{
				this.Position = this.Length;
				throw new BufferParseException("End of buffer reached prematurely.");
			}
		}

		private static int MaxUnicodeCharsSize
		{
			get
			{
				if (Reader.maxUnicodeCharsSize == 0)
				{
					Reader.maxUnicodeCharsSize = Encoding.Unicode.GetMaxCharCount(128);
				}
				return Reader.maxUnicodeCharsSize;
			}
		}

		private static int MaxAsciiCharsSize
		{
			get
			{
				if (Reader.maxAsciiCharsSize == 0)
				{
					Reader.maxAsciiCharsSize = CTSGlobals.AsciiEncoding.GetMaxCharCount(128);
				}
				return Reader.maxAsciiCharsSize;
			}
		}

		private bool TryReadHasValue(WireFormatStyle wireFormatStyle)
		{
			return wireFormatStyle != WireFormatStyle.Nspi || this.ReadBool();
		}

		private object ReadPropertyValueForType(PropertyType propertyType, WireFormatStyle wireFormatStyle)
		{
			bool flag = wireFormatStyle == WireFormatStyle.Nspi;
			if (propertyType <= PropertyType.Binary)
			{
				if (propertyType <= PropertyType.SysTime)
				{
					switch (propertyType)
					{
					case PropertyType.Unspecified:
					case (PropertyType)8:
					case (PropertyType)9:
					case (PropertyType)12:
					case (PropertyType)14:
					case (PropertyType)15:
					case (PropertyType)16:
					case (PropertyType)17:
					case (PropertyType)18:
					case (PropertyType)19:
						break;
					case PropertyType.Null:
						return null;
					case PropertyType.Int16:
						return this.ReadInt16();
					case PropertyType.Int32:
						return this.ReadInt32();
					case PropertyType.Float:
						return this.ReadSingle();
					case PropertyType.Double:
					case PropertyType.AppTime:
						return this.ReadDouble();
					case PropertyType.Currency:
					case PropertyType.Int64:
						return this.ReadInt64();
					case PropertyType.Error:
						return (ErrorCode)this.ReadUInt32();
					case PropertyType.Bool:
						return this.ReadBool();
					case PropertyType.Object:
						if (wireFormatStyle != WireFormatStyle.Nspi)
						{
							throw new NotImplementedException(string.Format("Property type not implemented: {0}.", propertyType));
						}
						goto IL_3B1;
					default:
						switch (propertyType)
						{
						case PropertyType.String8:
							if (this.TryReadHasValue(wireFormatStyle))
							{
								return this.ReadString8(StringFlags.IncludeNull);
							}
							goto IL_3B1;
						case PropertyType.Unicode:
							if (this.TryReadHasValue(wireFormatStyle))
							{
								return this.ReadUnicodeString(StringFlags.IncludeNull);
							}
							goto IL_3B1;
						default:
							if (propertyType == PropertyType.SysTime)
							{
								long fileTimeAsInt = this.ReadInt64();
								return PropertyValue.ExDateTimeFromFileTimeUtc(fileTimeAsInt);
							}
							break;
						}
						break;
					}
				}
				else if (propertyType != PropertyType.Guid)
				{
					switch (propertyType)
					{
					case PropertyType.ServerId:
						break;
					case (PropertyType)252:
						goto IL_39B;
					case PropertyType.Restriction:
						if (this.TryReadHasValue(wireFormatStyle))
						{
							return Restriction.Parse(this, wireFormatStyle);
						}
						goto IL_3B1;
					case PropertyType.Actions:
						if (wireFormatStyle == WireFormatStyle.Nspi)
						{
							throw new NotSupportedException(string.Format("Property type not supported: {0}.", propertyType));
						}
						return RuleAction.Parse(this);
					default:
						if (propertyType != PropertyType.Binary)
						{
							goto IL_39B;
						}
						break;
					}
					if (this.TryReadHasValue(wireFormatStyle))
					{
						uint count = this.ReadCountOrSize(wireFormatStyle);
						return this.ReadBytes(count);
					}
					goto IL_3B1;
				}
				else
				{
					if (this.TryReadHasValue(wireFormatStyle))
					{
						return this.ReadGuid();
					}
					goto IL_3B1;
				}
			}
			else if (propertyType <= PropertyType.MultiValueUnicode)
			{
				switch (propertyType)
				{
				case PropertyType.MultiValueInt16:
					if (this.TryReadHasValue(wireFormatStyle))
					{
						return this.ReadPropertyMultiValueForTagInternal<short>(PropertyType.Int16, flag ? 1U : 2U, wireFormatStyle);
					}
					goto IL_3B1;
				case PropertyType.MultiValueInt32:
					if (this.TryReadHasValue(wireFormatStyle))
					{
						return this.ReadPropertyMultiValueForTagInternal<int>(PropertyType.Int32, flag ? 1U : 4U, wireFormatStyle);
					}
					goto IL_3B1;
				case PropertyType.MultiValueFloat:
					if (this.TryReadHasValue(wireFormatStyle))
					{
						return this.ReadPropertyMultiValueForTagInternal<float>(PropertyType.Float, flag ? 1U : 4U, wireFormatStyle);
					}
					goto IL_3B1;
				case PropertyType.MultiValueDouble:
					if (this.TryReadHasValue(wireFormatStyle))
					{
						return this.ReadPropertyMultiValueForTagInternal<double>(PropertyType.Double, flag ? 1U : 8U, wireFormatStyle);
					}
					goto IL_3B1;
				case PropertyType.MultiValueCurrency:
					if (this.TryReadHasValue(wireFormatStyle))
					{
						return this.ReadPropertyMultiValueForTagInternal<long>(PropertyType.Currency, flag ? 1U : 8U, wireFormatStyle);
					}
					goto IL_3B1;
				case PropertyType.MultiValueAppTime:
					if (this.TryReadHasValue(wireFormatStyle))
					{
						return this.ReadPropertyMultiValueForTagInternal<double>(PropertyType.AppTime, flag ? 1U : 8U, wireFormatStyle);
					}
					goto IL_3B1;
				default:
					if (propertyType != PropertyType.MultiValueInt64)
					{
						switch (propertyType)
						{
						case PropertyType.MultiValueString8:
							if (this.TryReadHasValue(wireFormatStyle))
							{
								return this.ReadPropertyMultiValueForTagInternal<String8>(PropertyType.String8, 1U, wireFormatStyle);
							}
							goto IL_3B1;
						case PropertyType.MultiValueUnicode:
							if (this.TryReadHasValue(wireFormatStyle))
							{
								return this.ReadPropertyMultiValueForTagInternal<string>(PropertyType.Unicode, flag ? 1U : 2U, wireFormatStyle);
							}
							goto IL_3B1;
						}
					}
					else
					{
						if (this.TryReadHasValue(wireFormatStyle))
						{
							return this.ReadPropertyMultiValueForTagInternal<long>(PropertyType.Int64, flag ? 1U : 8U, wireFormatStyle);
						}
						goto IL_3B1;
					}
					break;
				}
			}
			else if (propertyType != PropertyType.MultiValueSysTime)
			{
				if (propertyType != PropertyType.MultiValueGuid)
				{
					if (propertyType == PropertyType.MultiValueBinary)
					{
						if (this.TryReadHasValue(wireFormatStyle))
						{
							return this.ReadPropertyMultiValueForTagInternal<byte[]>(PropertyType.Binary, flag ? 1U : 2U, wireFormatStyle);
						}
						goto IL_3B1;
					}
				}
				else
				{
					if (this.TryReadHasValue(wireFormatStyle))
					{
						return this.ReadPropertyMultiValueForTagInternal<Guid>(PropertyType.Guid, flag ? 1U : 16U, wireFormatStyle);
					}
					goto IL_3B1;
				}
			}
			else
			{
				if (this.TryReadHasValue(wireFormatStyle))
				{
					return this.ReadPropertyMultiValueForTagInternal<ExDateTime>(PropertyType.SysTime, flag ? 1U : 8U, wireFormatStyle);
				}
				goto IL_3B1;
			}
			IL_39B:
			throw new NotSupportedException(string.Format("Property type not supported: {0}.", propertyType));
			IL_3B1:
			return null;
		}

		private T[] ReadPropertyMultiValueForTagInternal<T>(PropertyType elementPropertyType, uint minimumElementSize, WireFormatStyle wireFormatStyle)
		{
			uint num = this.ReadUInt32();
			this.CheckBoundary(num, minimumElementSize);
			T[] array = new T[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				object obj = this.ReadPropertyValueForType(elementPropertyType, wireFormatStyle);
				if (obj != null)
				{
					array[num2] = (T)((object)obj);
				}
				num2++;
			}
			return array;
		}

		private static Encoding EncodingWithDecoderFallback(Encoding encoding, StringFlags flags)
		{
			if (encoding == CTSGlobals.AsciiEncoding && (flags & StringFlags.FailOnError) == StringFlags.FailOnError)
			{
				Encoding encoding2 = (Encoding)CTSGlobals.AsciiEncoding.Clone();
				encoding2.DecoderFallback = new Reader.BufferParseExceptionFallback();
				return encoding2;
			}
			return encoding;
		}

		private const int MaxCharBytesSize = 128;

		private const char FallbackCharacter = '?';

		public const uint GuidSize = 16U;

		private static int maxUnicodeCharsSize;

		private static int maxAsciiCharsSize;

		private byte[] charBytes;

		private char[] charUnicodeBuffer;

		private sealed class BufferParseExceptionFallback : DecoderFallback
		{
			public override DecoderFallbackBuffer CreateFallbackBuffer()
			{
				return new Reader.BufferParseExceptionFallbackBuffer();
			}

			public override bool Equals(object value)
			{
				return value is Reader.BufferParseExceptionFallback;
			}

			public override int GetHashCode()
			{
				return 880;
			}

			public override int MaxCharCount
			{
				get
				{
					return 0;
				}
			}
		}

		public sealed class BufferParseExceptionFallbackBuffer : DecoderFallbackBuffer
		{
			public override bool Fallback(byte[] bytesUnknown, int index)
			{
				throw new BufferParseException("Contains an invalid character in this encoding");
			}

			public override char GetNextChar()
			{
				return '\0';
			}

			public override bool MovePrevious()
			{
				return false;
			}

			public override int Remaining
			{
				get
				{
					return 0;
				}
			}
		}
	}
}
