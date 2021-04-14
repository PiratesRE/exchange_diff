using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Internal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Asn1Reader : IDisposable
	{
		public Asn1Reader(Stream inputStream)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (!inputStream.CanRead)
			{
				throw new NotSupportedException("Strings.StreamDoesNotSupportRead");
			}
			this.InputStream = inputStream;
			this.readBuffer = new byte[4096];
			this.tagStack = new Asn1Reader.TagInfo[32];
			this.tagStackTop = -1;
			this.readState = Asn1Reader.ReadState.Begin;
		}

		public int StreamOffset
		{
			get
			{
				this.AssertGoodToUse(true);
				return this.bufferStreamOffset + this.readOffset;
			}
		}

		public int Depth
		{
			get
			{
				return this.tagStackTop + 1;
			}
		}

		public int SequenceNum
		{
			get
			{
				this.AssertCurrentTag();
				if (this.tagStackTop > 0)
				{
					return this.tagStack[this.tagStackTop - 1].SequenceNum;
				}
				return 0;
			}
		}

		public byte TagIdByte
		{
			get
			{
				this.AssertCurrentTag();
				return this.tagStack[this.tagStackTop].TagIdByte;
			}
		}

		public int TagNumber
		{
			get
			{
				this.AssertCurrentTag();
				return this.tagStack[this.tagStackTop].TagNumber;
			}
		}

		public bool IsConstructedTag
		{
			get
			{
				this.AssertCurrentTag();
				return 0 != (this.tagStack[this.tagStackTop].TagIdByte & 32);
			}
		}

		public TagClass TagClass
		{
			get
			{
				this.AssertCurrentTag();
				return (TagClass)(this.tagStack[this.tagStackTop].TagIdByte & 192);
			}
		}

		public EncodingType EncodingType
		{
			get
			{
				this.AssertCurrentTag();
				return this.tagStack[this.tagStackTop].EncodingType;
			}
		}

		public int ValueLength
		{
			get
			{
				this.AssertCurrentTag();
				return this.tagStack[this.tagStackTop].ValueLength;
			}
		}

		public bool IsLongValue
		{
			get
			{
				this.AssertCurrentTag();
				return this.ValueLength < 0 || this.ValueLength > 1048576;
			}
		}

		public int ValueStreamOffset
		{
			get
			{
				this.AssertCurrentTag();
				return this.tagStack[this.tagStackTop].ValueStreamOffset;
			}
		}

		private bool TagIsEndOfContentMarker
		{
			get
			{
				return this.tagStack[this.tagStackTop].TagIdByte == 0 && this.tagStack[this.tagStackTop].ValueLength == 0;
			}
		}

		private Decoder AsciiDecoder
		{
			get
			{
				if (this.asciiDecoder == null)
				{
					this.asciiDecoder = CTSGlobals.AsciiEncoding.GetDecoder();
				}
				return this.asciiDecoder;
			}
		}

		private Decoder Utf8Decoder
		{
			get
			{
				if (this.utf8Decoder == null)
				{
					this.utf8Decoder = Encoding.UTF8.GetDecoder();
				}
				return this.utf8Decoder;
			}
		}

		private Decoder UnicodeDecoder
		{
			get
			{
				if (this.unicodeDecoder == null)
				{
					this.unicodeDecoder = Encoding.Unicode.GetDecoder();
				}
				return this.unicodeDecoder;
			}
		}

		private Decoder Utf32Decoder
		{
			get
			{
				if (this.utf32Decoder == null)
				{
					this.utf32Decoder = Encoding.UTF32.GetDecoder();
				}
				return this.utf32Decoder;
			}
		}

		public bool ReadNext()
		{
			this.AssertGoodToUse(true);
			if (this.readState != Asn1Reader.ReadState.Begin)
			{
				if (this.tagStackTop < 0)
				{
					return false;
				}
				if (!this.IsConstructedTag)
				{
					this.SkipValue();
				}
			}
			for (;;)
			{
				if (this.readState != Asn1Reader.ReadState.Begin && this.ValueLength >= 0 && this.StreamOffset >= this.ValueStreamOffset + this.ValueLength)
				{
					if (!this.PopTag())
					{
						break;
					}
				}
				else
				{
					this.ReadAndPushTag();
					if (!this.TagIsEndOfContentMarker)
					{
						goto IL_9C;
					}
					this.PopTag();
					if (this.ValueLength != -1)
					{
						goto Block_9;
					}
					if (!this.PopTag())
					{
						goto Block_10;
					}
				}
			}
			this.readState = Asn1Reader.ReadState.EndOfFile;
			return false;
			Block_9:
			throw new ExchangeDataException("invalid data - end-of-content marker for a tag with definite length");
			Block_10:
			this.readState = Asn1Reader.ReadState.EndOfFile;
			return false;
			IL_9C:
			this.readState = Asn1Reader.ReadState.BeginValue;
			return true;
		}

		public bool ReadFirstChild()
		{
			this.AssertGoodToUse(true);
			if (this.readState != Asn1Reader.ReadState.BeginValue || !this.IsConstructedTag)
			{
				throw new InvalidOperationException("not at the beginning of tag content or tag is not constructed");
			}
			if (this.ValueLength >= 0 && this.StreamOffset >= this.ValueStreamOffset + this.ValueLength)
			{
				this.readState = Asn1Reader.ReadState.ReadValue;
				return false;
			}
			this.ReadAndPushTag();
			if (!this.TagIsEndOfContentMarker)
			{
				this.readState = Asn1Reader.ReadState.BeginValue;
				return true;
			}
			this.PopTag();
			if (this.ValueLength != -1)
			{
				throw new ExchangeDataException("invalid data - end-of-content marker for a tag with definite length");
			}
			this.tagStack[this.tagStackTop].ValueLength = this.StreamOffset - this.ValueStreamOffset;
			this.readState = Asn1Reader.ReadState.ReadValue;
			return false;
		}

		public bool ReadNextSibling()
		{
			this.AssertGoodToUse(true);
			if (this.tagStackTop < 0)
			{
				throw new InvalidOperationException("not inside a tag");
			}
			if (this.readState == Asn1Reader.ReadState.BeginValue || this.readState == Asn1Reader.ReadState.ReadValue)
			{
				if (this.ValueLength >= 0)
				{
					this.SkipValue();
				}
				else
				{
					this.SkipTagsToMarker();
				}
			}
			if (!this.PopTag())
			{
				return false;
			}
			if (this.ValueLength >= 0 && this.StreamOffset >= this.ValueStreamOffset + this.ValueLength)
			{
				this.readState = Asn1Reader.ReadState.ReadValue;
				return false;
			}
			this.ReadAndPushTag();
			if (!this.TagIsEndOfContentMarker)
			{
				this.readState = Asn1Reader.ReadState.BeginValue;
				return true;
			}
			this.PopTag();
			if (this.ValueLength != -1)
			{
				throw new ExchangeDataException("invalid data - end-of-content marker for a tag with definite length");
			}
			this.tagStack[this.tagStackTop].ValueLength = this.StreamOffset - this.ValueStreamOffset;
			this.readState = Asn1Reader.ReadState.ReadValue;
			return false;
		}

		public bool ReadValueAsBool()
		{
			this.AssertAtTheBeginningOfValue();
			if (this.EncodingType != EncodingType.Boolean && (this.EncodingType != EncodingType.Unknown || this.IsConstructedTag))
			{
				throw new InvalidOperationException("cannot convert value");
			}
			if (this.ValueLength != 1)
			{
				throw new ExchangeDataException("invalid data - invalid boolean value encoding");
			}
			if (!this.EnsureMoreDataLoaded(this.ValueLength))
			{
				throw new ExchangeDataException("invalid data - premature end of file");
			}
			this.readState = Asn1Reader.ReadState.ReadValue;
			return 0 != this.ReadByte();
		}

		public int ReadValueAsInt()
		{
			this.AssertAtTheBeginningOfValue();
			if (this.EncodingType != EncodingType.Integer && this.EncodingType != EncodingType.Enumerated && (this.EncodingType != EncodingType.Unknown || this.IsConstructedTag))
			{
				throw new InvalidOperationException("cannot convert value");
			}
			if (this.ValueLength > 4)
			{
				throw new ExchangeDataException("invalid data - integer value overflow");
			}
			if (!this.EnsureMoreDataLoaded(this.ValueLength))
			{
				throw new ExchangeDataException("invalid data - premature end of file");
			}
			this.readState = Asn1Reader.ReadState.ReadValue;
			return this.ReadInteger(this.ValueLength);
		}

		public long ReadValueAsLong()
		{
			this.AssertAtTheBeginningOfValue();
			if (this.EncodingType != EncodingType.Integer && (this.EncodingType != EncodingType.Unknown || this.IsConstructedTag))
			{
				throw new InvalidOperationException("cannot convert value");
			}
			if (this.ValueLength > 8)
			{
				throw new ExchangeDataException("invalid data - long value overflow");
			}
			if (!this.EnsureMoreDataLoaded(this.ValueLength))
			{
				throw new ExchangeDataException("invalid data - premature end of file");
			}
			this.readState = Asn1Reader.ReadState.ReadValue;
			return this.ReadLong(this.ValueLength);
		}

		public OID ReadValueAsOID()
		{
			this.AssertAtTheBeginningOfValue();
			if (this.EncodingType != EncodingType.ObjectIdentifier && (this.EncodingType != EncodingType.Unknown || this.IsConstructedTag))
			{
				throw new InvalidOperationException("cannot convert value");
			}
			if (this.ValueLength > 1024)
			{
				throw new ExchangeDataException("inalid data - object identifier is too long");
			}
			if (!this.EnsureMoreDataLoaded(this.ValueLength))
			{
				throw new ExchangeDataException("invalid data - premature end of file");
			}
			this.readState = Asn1Reader.ReadState.ReadValue;
			return new OID(this.readBuffer, this.readOffset, this.ValueLength);
		}

		public byte[] ReadValueAsByteArray()
		{
			this.AssertAtTheBeginningOfValue();
			if (this.IsLongValue)
			{
				throw new InvalidOperationException("this value can be read only with a stream");
			}
			byte[] array = new byte[this.ValueLength];
			this.ReadRawValue(array, 0, array.Length, false);
			return array;
		}

		public string ReadValueAsString()
		{
			this.AssertAtTheBeginningOfValue();
			if (this.IsLongValue)
			{
				throw new InvalidOperationException("this value can be read only with a stream");
			}
			if (this.decodeBuffer == null)
			{
				this.decodeBuffer = new char[512];
			}
			int num = this.ReadTextValue(this.decodeBuffer, 0, this.decodeBuffer.Length);
			if (this.StreamOffset != this.ValueStreamOffset + this.ValueLength || !this.decoderFlushed)
			{
				StringBuilder stringBuilder = new StringBuilder();
				do
				{
					stringBuilder.Append(this.decodeBuffer, 0, num);
					num = this.ReadTextValue(this.decodeBuffer, 0, this.decodeBuffer.Length);
				}
				while (num != 0);
				return stringBuilder.ToString();
			}
			if (num == 0)
			{
				return string.Empty;
			}
			return new string(this.decodeBuffer, 0, num);
		}

		public int ReadTextValue(char[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Strings.OffsetOutOfRange");
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Strings.CountOutOfRange");
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", "Strings.CountTooLarge");
			}
			this.AssertGoodToUse(true);
			this.AssertCurrentTag();
			if (this.EncodingType != EncodingType.Utf8String && this.EncodingType != EncodingType.NumericString && this.EncodingType != EncodingType.PrintableString && this.EncodingType != EncodingType.TeletexString && this.EncodingType != EncodingType.VideotexString && this.EncodingType != EncodingType.IA5String && this.EncodingType != EncodingType.UtcTime && this.EncodingType != EncodingType.GeneralizedTime && this.EncodingType != EncodingType.GraphicString && this.EncodingType != EncodingType.VisibleString && this.EncodingType != EncodingType.GeneralString && this.EncodingType != EncodingType.UniversalString && this.EncodingType != EncodingType.BMPString && (this.EncodingType != EncodingType.Unknown || this.IsConstructedTag))
			{
				throw new InvalidOperationException("cannot convert value");
			}
			if (this.IsConstructedTag)
			{
				throw new InvalidOperationException("NYI");
			}
			if (this.readState == Asn1Reader.ReadState.BeginValue)
			{
				if (this.EncodingType == EncodingType.Utf8String)
				{
					this.decoder = this.Utf8Decoder;
				}
				else if (this.EncodingType == EncodingType.BMPString)
				{
					this.decoder = this.UnicodeDecoder;
				}
				else if (this.EncodingType == EncodingType.UniversalString)
				{
					this.decoder = this.Utf32Decoder;
				}
				else
				{
					this.decoder = this.AsciiDecoder;
				}
				this.decoder.Reset();
				this.decoderFlushed = false;
				this.readState = Asn1Reader.ReadState.ReadValue;
			}
			else if (this.decoder == null)
			{
				throw new InvalidOperationException("Strings.ReaderInvalidOperationPropTextAfterRaw");
			}
			int num = 0;
			while ((this.StreamOffset != this.ValueStreamOffset + this.ValueLength || !this.decoderFlushed) && count > 12)
			{
				if (!this.EnsureMoreDataLoaded(1))
				{
					throw new ExchangeDataException("invalid data - premature end of file");
				}
				int num2 = Math.Min(this.AvailableCount(), this.ValueStreamOffset + this.ValueLength - this.StreamOffset);
				int count2;
				int num3;
				this.decoder.Convert(this.readBuffer, this.readOffset, num2, buffer, offset, count, num2 == this.ValueStreamOffset + this.ValueLength - this.StreamOffset, out count2, out num3, out this.decoderFlushed);
				this.SkipBytes(count2);
				offset += num3;
				count -= num3;
				num += num3;
			}
			this.readState = Asn1Reader.ReadState.ReadValue;
			return num;
		}

		public int ReadBytesValue(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Strings.OffsetOutOfRange");
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Strings.CountOutOfRange");
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", "Strings.CountTooLarge");
			}
			return this.ReadRawValue(buffer, offset, count, false);
		}

		public void SetEncodingType(EncodingType encodingType)
		{
			this.tagStack[this.tagStackTop].EncodingType = encodingType;
		}

		public void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void AssertGoodToUse(bool affectsChild)
		{
			if (this.InputStream == null)
			{
				throw new ObjectDisposedException("Asn1Reader");
			}
		}

		internal void AssertCurrentTag()
		{
			if (this.readState < Asn1Reader.ReadState.BeginValue)
			{
				throw new InvalidOperationException("Strings.ReaderInvalidOperationMustBeInTagValue");
			}
		}

		internal void AssertAtTheBeginningOfValue()
		{
			if (this.readState != Asn1Reader.ReadState.BeginValue)
			{
				throw new InvalidOperationException("Strings.ReaderInvalidOperationMustBeAtTheBeginningOfTagValue");
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.InputStream == null)
				{
					return;
				}
				this.InputStream.Flush();
				this.InputStream.Dispose();
			}
			this.InputStream = null;
			this.readBuffer = null;
			this.decoder = null;
			this.unicodeDecoder = null;
			this.utf8Decoder = null;
			this.utf32Decoder = null;
			this.asciiDecoder = null;
		}

		private bool ReadAndPushTag()
		{
			int num = this.tagStackTop + 1;
			if (num == this.tagStack.Length)
			{
				if (this.tagStack.Length >= 4096)
				{
					throw new ExchangeDataException("invalid data - nesting too deep");
				}
				Asn1Reader.TagInfo[] destinationArray = new Asn1Reader.TagInfo[this.tagStack.Length * 2];
				Array.Copy(this.tagStack, 0, destinationArray, 0, this.tagStack.Length);
				this.tagStack = destinationArray;
			}
			if (!this.EnsureMoreDataLoaded(2))
			{
				throw new ExchangeDataException("invalid data - premature end of file");
			}
			this.tagStack[num].TagIdByte = this.ReadByte();
			this.tagStack[num].EncodingType = EncodingType.Unknown;
			this.tagStack[num].TagNumber = (int)(this.tagStack[num].TagIdByte & 31);
			if (this.tagStack[num].TagIdByte < 64)
			{
				if (!Asn1Reader.ValidUniversalTags[(int)this.tagStack[num].TagIdByte])
				{
					throw new ExchangeDataException("invalid universal tag");
				}
				this.tagStack[num].EncodingType = (EncodingType)(this.tagStack[num].TagIdByte & 31);
			}
			if (this.tagStack[num].TagNumber == 31)
			{
				this.tagStack[num].TagNumber = this.ReadLongTagNumber();
			}
			if (!this.EnsureMoreDataLoaded(1))
			{
				throw new ExchangeDataException("invalid data - premature end of file");
			}
			byte b = this.ReadByte();
			if ((b & 128) == 0)
			{
				this.tagStack[num].ValueLength = (int)b;
			}
			else if (b == 128)
			{
				this.tagStack[num].ValueLength = -1;
				if ((this.tagStack[num].TagIdByte & 32) == 0)
				{
					throw new ExchangeDataException("invalid data - indefinite length for primitive type");
				}
			}
			else
			{
				int num2 = (int)(b & 127);
				if (num2 == 127)
				{
					throw new ExchangeDataException("invalid data - invalid length value");
				}
				if (num2 > 4)
				{
					throw new ExchangeDataException("invalid data - length field does not fit into integer");
				}
				if (!this.EnsureMoreDataLoaded(num2))
				{
					throw new ExchangeDataException("invalid data - premature end of file");
				}
				this.tagStack[num].ValueLength = this.ReadUnsignedInteger(num2);
				if (this.tagStack[num].ValueLength < 0 || this.StreamOffset + this.tagStack[num].ValueLength < this.StreamOffset)
				{
					throw new ExchangeDataException("invalid data - negative or extremely large content length");
				}
			}
			if ((this.tagStack[num].TagIdByte == 0 || this.tagStack[num].TagIdByte == 5) && this.tagStack[num].ValueLength != 0)
			{
				throw new ExchangeDataException("invalid length for null or end of content");
			}
			this.tagStack[num].ValueStreamOffset = this.StreamOffset;
			this.tagStack[num].SequenceNum = 0;
			if (this.tagStackTop >= 0)
			{
				Asn1Reader.TagInfo[] array = this.tagStack;
				int num3 = this.tagStackTop;
				array[num3].SequenceNum = array[num3].SequenceNum + 1;
			}
			if (this.tagStackTop >= 0 && this.ValueLength >= 0 && this.tagStack[num].ValueLength >= 0 && (long)this.tagStack[num].ValueStreamOffset + (long)this.tagStack[num].ValueLength > (long)this.ValueStreamOffset + (long)this.ValueLength)
			{
				throw new ExchangeDataException("nested tag overflows its parent");
			}
			this.tagStackTop++;
			return true;
		}

		private bool PopTag()
		{
			this.tagStackTop--;
			if (this.tagStackTop < 0)
			{
				return false;
			}
			if (this.ValueLength >= 0 && this.StreamOffset > this.ValueStreamOffset + this.ValueLength)
			{
				throw new ExchangeDataException("invalid data - tag overflows its parent");
			}
			return true;
		}

		private void PushBackTag()
		{
			this.tagStackTop++;
		}

		private int ReadRawValue(byte[] buffer, int offset, int count, bool fromWrapper)
		{
			this.AssertGoodToUse(!fromWrapper);
			if (this.readState == Asn1Reader.ReadState.BeginValue)
			{
				if (this.IsConstructedTag)
				{
					throw new InvalidOperationException("NYI");
				}
				this.decoder = null;
				this.readState = Asn1Reader.ReadState.ReadValue;
			}
			else
			{
				if (this.readState != Asn1Reader.ReadState.ReadValue)
				{
					throw new InvalidOperationException("must be inside value");
				}
				if (this.decoder != null)
				{
					throw new InvalidOperationException("continue reading value as text");
				}
			}
			int num = 0;
			while (this.StreamOffset < this.ValueStreamOffset + this.ValueLength && count != 0)
			{
				if (!this.EnsureMoreDataLoaded(1))
				{
					throw new ExchangeDataException("invalid data - premature end of file");
				}
				int num2 = Math.Min(count, Math.Min(this.ValueStreamOffset + this.ValueLength - this.StreamOffset, this.AvailableCount()));
				this.ReadBytes(buffer, offset, num2);
				offset += num2;
				count -= num2;
				num += num2;
			}
			return num;
		}

		private void SkipTagsToMarker()
		{
			int num = this.tagStackTop;
			for (;;)
			{
				if (this.ValueLength >= 0 && this.StreamOffset >= this.ValueStreamOffset + this.ValueLength)
				{
					this.PopTag();
				}
				else
				{
					this.ReadAndPushTag();
					if (this.TagIsEndOfContentMarker)
					{
						this.PopTag();
						if (this.tagStackTop == num)
						{
							break;
						}
					}
					else if (this.ValueLength >= 0)
					{
						this.SkipValue();
						this.PopTag();
					}
				}
			}
		}

		private void SkipValue()
		{
			if (this.StreamOffset < this.ValueStreamOffset + this.ValueLength)
			{
				int num2;
				for (int num = this.ValueStreamOffset + this.ValueLength - this.StreamOffset; num != 0; num -= num2)
				{
					if (!this.EnsureMoreDataLoaded(1))
					{
						throw new ExchangeDataException("invalid data - premature end of file");
					}
					num2 = Math.Min(num, this.AvailableCount());
					this.SkipBytes(num2);
				}
			}
		}

		private int AvailableCount()
		{
			return this.readEnd - this.readOffset;
		}

		private bool EnsureMoreDataLoaded(int bytes)
		{
			return !this.NeedToLoadMoreData(bytes) || this.LoadMoreData(bytes);
		}

		private bool NeedToLoadMoreData(int bytes)
		{
			return this.AvailableCount() < bytes;
		}

		private bool LoadMoreData(int bytes)
		{
			if (this.endOfFile)
			{
				return false;
			}
			if (this.readBuffer.Length < bytes)
			{
				byte[] dst = new byte[Math.Max(this.readBuffer.Length * 2, bytes)];
				if (this.readEnd - this.readOffset != 0)
				{
					Buffer.BlockCopy(this.readBuffer, this.readOffset, dst, 0, this.readEnd - this.readOffset);
				}
				this.readBuffer = dst;
			}
			else if (this.readEnd - this.readOffset != 0 && this.readOffset != 0)
			{
				Buffer.BlockCopy(this.readBuffer, this.readOffset, this.readBuffer, 0, this.readEnd - this.readOffset);
			}
			int num = this.readOffset;
			this.readEnd -= this.readOffset;
			this.readOffset = 0;
			this.bufferStreamOffset += num;
			int num2 = this.InputStream.Read(this.readBuffer, this.readEnd, this.readBuffer.Length - this.readEnd);
			this.readEnd += num2;
			this.endOfFile = (num2 == 0);
			return this.readEnd >= bytes;
		}

		private byte ReadByte()
		{
			byte result = this.readBuffer[this.readOffset];
			this.readOffset++;
			return result;
		}

		private int ReadInteger(int length)
		{
			int num = 0;
			if (length != 0)
			{
				num = this.readBuffer[this.readOffset++] << 24 >> 24;
				while (--length != 0)
				{
					num = (num << 8 | (int)this.readBuffer[this.readOffset++]);
				}
			}
			return num;
		}

		private long ReadLong(int length)
		{
			long num = 0L;
			if (length != 0)
			{
				num = (long)(this.readBuffer[this.readOffset++] << 24 >> 24);
				while (--length != 0)
				{
					num = (num << 8 | (long)((ulong)this.readBuffer[this.readOffset++]));
				}
			}
			return num;
		}

		private int ReadUnsignedInteger(int length)
		{
			int num = 0;
			if (length != 0)
			{
				num = (int)this.readBuffer[this.readOffset++];
				while (--length != 0)
				{
					num = (num << 8 | (int)this.readBuffer[this.readOffset++]);
				}
			}
			return num;
		}

		private int ReadLongTagNumber()
		{
			int num = 0;
			while (this.EnsureMoreDataLoaded(1))
			{
				if (num >= 16777216)
				{
					throw new ExchangeDataException("invalid data - tag number is too big");
				}
				byte b = this.ReadByte();
				num = (num << 7) + (int)(b & 127);
				if ((b & 128) == 0)
				{
					return num;
				}
			}
			throw new ExchangeDataException("invalid data - premature end of file");
		}

		private void ReadBytes(byte[] buffer, int offset, int count)
		{
			Buffer.BlockCopy(this.readBuffer, this.readOffset, buffer, offset, count);
			this.readOffset += count;
		}

		private void SkipBytes(int count)
		{
			this.readOffset += count;
		}

		private const int ReadBufferSize = 4096;

		private const byte TagClassMask = 192;

		private const byte TagFormConstructed = 32;

		private const byte TagNumberMask = 31;

		private const byte TagNumberLong = 31;

		private const byte TagNumberContinuationMask = 127;

		private const byte TagNumberContinuationSentinel = 128;

		private const byte LengthLong = 128;

		private const byte LengthMask = 127;

		private const byte LengthIndefinite = 128;

		internal Stream InputStream;

		private static readonly bool[] ValidUniversalTags = new bool[]
		{
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			false,
			true,
			true,
			false,
			true,
			true,
			false,
			false,
			false,
			false,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			false,
			true,
			false,
			false,
			false,
			false,
			true,
			true,
			false,
			false,
			true,
			true,
			false,
			false,
			true,
			true,
			false,
			false,
			false,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			false
		};

		private byte[] readBuffer;

		private int readOffset;

		private int readEnd;

		private bool endOfFile;

		private int bufferStreamOffset;

		private Asn1Reader.ReadState readState;

		private Asn1Reader.TagInfo[] tagStack;

		private int tagStackTop;

		private bool decoderFlushed;

		private Decoder decoder;

		private char[] decodeBuffer;

		private Decoder asciiDecoder;

		private Decoder utf8Decoder;

		private Decoder unicodeDecoder;

		private Decoder utf32Decoder;

		internal enum ReadState
		{
			EndOfFile,
			Begin,
			BeginValue,
			ReadValue
		}

		private struct TagInfo
		{
			public byte TagIdByte;

			public EncodingType EncodingType;

			public int TagNumber;

			public int ValueLength;

			public int ValueStreamOffset;

			public int SequenceNum;
		}
	}
}
