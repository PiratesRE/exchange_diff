using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public class MimeParameter : MimeNode
	{
		public MimeParameter(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.paramName = Header.NormalizeString(name);
		}

		public MimeParameter(string name, string value) : this(name)
		{
			this.decodedValue = value;
		}

		public string Name
		{
			get
			{
				return this.paramName;
			}
		}

		public string Value
		{
			get
			{
				DecodingResults decodingResults;
				if (this.decodedValue == null && !this.TryGetValue(base.GetHeaderDecodingOptions(), out decodingResults, out this.decodedValue))
				{
					MimeCommon.ThrowDecodingFailedException(ref decodingResults);
				}
				return this.decodedValue;
			}
			set
			{
				if (this.segmentNumber == 0)
				{
					base.RemoveAll();
					this.segmentNumber = -1;
				}
				else if (0 < this.segmentNumber)
				{
					throw new NotSupportedException(Strings.CantSetValueOfRfc2231ContinuationSegment);
				}
				this.RawValue = null;
				this.decodedValue = value;
			}
		}

		internal byte[] RawValue
		{
			get
			{
				if (this.valueFragments.Length == 0 && this.decodedValue != null && 0 < this.decodedValue.Length)
				{
					bool flag = this.paramName == "charset";
					this.valueFragments = this.EncodeValue(this.decodedValue, flag ? MimeParameter.EncodingOptionsAscii : base.GetDocumentEncodingOptions());
				}
				return this.valueFragments.GetSz(4026531839U);
			}
			set
			{
				if (this.segmentNumber == 0)
				{
					base.RemoveAll();
					this.segmentNumber = -1;
				}
				else if (0 < this.segmentNumber)
				{
					throw new NotSupportedException(Strings.CantSetRawValueOfRfc2231ContinuationSegment);
				}
				this.decodedValue = null;
				this.valueFragments.Reset();
				this.valueEncoded = false;
				if (value != null && 0 < value.Length)
				{
					this.valueFragments.AppendFragment(new MimeString(value));
				}
				this.SetDirty();
			}
		}

		internal int RawLength
		{
			get
			{
				return this.valueFragments.GetLength(4026531839U);
			}
		}

		internal bool ValueEncoded
		{
			set
			{
				this.valueEncoded = value;
			}
		}

		internal int SegmentNumber
		{
			get
			{
				return this.segmentNumber;
			}
			set
			{
				this.segmentNumber = value;
			}
		}

		internal bool AllowAppend
		{
			set
			{
				this.allowAppend = value;
			}
		}

		public sealed override MimeNode Clone()
		{
			MimeParameter mimeParameter = new MimeParameter(this.paramName);
			this.CopyTo(mimeParameter);
			return mimeParameter;
		}

		public sealed override void CopyTo(object destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destination == this)
			{
				return;
			}
			MimeParameter mimeParameter = destination as MimeParameter;
			if (mimeParameter == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			mimeParameter.AllowAppend = true;
			base.CopyTo(destination);
			mimeParameter.AllowAppend = false;
			mimeParameter.valueEncoded = this.valueEncoded;
			mimeParameter.segmentNumber = this.segmentNumber;
			mimeParameter.valueFragments = this.valueFragments.Clone();
			mimeParameter.decodedValue = this.decodedValue;
			mimeParameter.paramName = this.paramName;
		}

		public bool TryGetValue(out string value)
		{
			DecodingResults decodingResults;
			return this.TryGetValue(base.GetHeaderDecodingOptions(), out decodingResults, out value);
		}

		public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
		{
			if (decodingOptions.Charset == null)
			{
				decodingOptions.Charset = base.GetDefaultHeaderDecodingCharset(null, null);
			}
			if ((DecodingFlags.Rfc2231 & decodingOptions.DecodingFlags) != DecodingFlags.None)
			{
				if (this.segmentNumber == 0)
				{
					return this.TryDecodeRfc2231(ref decodingOptions, out decodingResults, out value);
				}
				if (0 < this.segmentNumber)
				{
					throw new NotSupportedException(Strings.CantGetValueOfRfc2231ContinuationSegment);
				}
			}
			if (this.valueFragments.Length == 0)
			{
				decodingResults = default(DecodingResults);
				value = ((this.decodedValue != null) ? this.decodedValue : string.Empty);
				return true;
			}
			return MimeCommon.TryDecodeValue(this.valueFragments, 4026531839U, decodingOptions, out decodingResults, out value);
		}

		internal static string CorrectValue(string value)
		{
			if (-1 == value.IndexOfAny(MimeParameter.ControlCharacters))
			{
				return value;
			}
			return MimeParameter.CorrectValue(value.ToCharArray());
		}

		internal static string CorrectValue(char[] value)
		{
			int num = MimeParameter.ControlCharacters.Length;
			for (int i = 0; i < value.Length; i++)
			{
				for (int j = 0; j < num; j++)
				{
					if (value[i] == MimeParameter.ControlCharacters[j])
					{
						value[i] = ' ';
						break;
					}
				}
			}
			return new string(value);
		}

		internal bool FallBackIfRequired(byte[] bytes, DecodingOptions decodingOptions, out string value)
		{
			if ((DecodingFlags.FallbackToRaw & decodingOptions.DecodingFlags) != DecodingFlags.None)
			{
				if (bytes == null)
				{
					value = string.Empty;
				}
				else
				{
					value = ByteString.BytesToString(bytes, decodingOptions.AllowUTF8);
					if ((DecodingFlags.AllowControlCharacters & decodingOptions.DecodingFlags) == DecodingFlags.None)
					{
						value = MimeParameter.CorrectValue(value);
					}
				}
				return true;
			}
			value = null;
			return false;
		}

		internal bool IsName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.paramName.Equals(name, StringComparison.OrdinalIgnoreCase);
		}

		internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			MimeStringList fragments = this.valueFragments;
			long num = 0L;
			if (this.valueFragments.Length == 0 && this.decodedValue != null && 0 < this.decodedValue.Length)
			{
				fragments = this.EncodeValue(this.decodedValue, encodingOptions);
				this.valueFragments = fragments;
			}
			else if ((byte)(EncodingFlags.ForceReencode & encodingOptions.EncodingFlags) != 0 && 0 >= this.segmentNumber)
			{
				fragments = this.EncodeValue(this.Value, encodingOptions);
			}
			bool flag = false;
			if (this.IsQuotingReqired() || fragments.Length == 0)
			{
				flag = true;
			}
			else
			{
				for (int i = 0; i < fragments.Count; i++)
				{
					MimeString str = fragments[i];
					int num2 = 0;
					int num3 = ValueParser.ParseToken(str, out num2, encodingOptions.AllowUTF8);
					if (268435456U != str.Mask && str.Length != num3)
					{
						flag = true;
						break;
					}
				}
			}
			MimeNode mimeNode = null;
			if (this.segmentNumber == 0)
			{
				mimeNode = base.FirstChild;
				while (mimeNode != null && !(mimeNode is MimeParameter))
				{
					mimeNode = mimeNode.NextSibling;
				}
			}
			MimeString mimeString = ((this.segmentNumber == 0 && mimeNode != null) || 0 < this.segmentNumber) ? new MimeString(this.segmentNumber.ToString()) : default(MimeString);
			if (1 < currentLineLength.InChars)
			{
				int num4 = 1 + this.paramName.Length + 1;
				byte[] sz = fragments.GetSz();
				int num5 = ByteString.BytesToCharCount(sz, encodingOptions.AllowUTF8);
				if (mimeString.Length != 0)
				{
					num4 += 1 + mimeString.Length;
				}
				if (this.valueEncoded)
				{
					num4++;
				}
				int num6 = num5;
				if (flag)
				{
					num6 += 2;
				}
				if (base.NextSibling != null)
				{
					if (num5 == 0)
					{
						num4++;
					}
					else
					{
						num6++;
					}
				}
				num6 += num4;
				if (currentLineLength.InChars + num6 > 78)
				{
					num += Header.WriteLineEnd(stream, ref currentLineLength);
					stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
					num += (long)Header.LineStartWhitespace.Length;
					currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
				}
				else
				{
					stream.Write(MimeString.Space, 0, MimeString.Space.Length);
					num += (long)MimeString.Space.Length;
					currentLineLength.IncrementBy(MimeString.Space.Length);
				}
			}
			int num7 = ByteString.StringToBytesCount(this.paramName, false);
			if (scratchBuffer == null || scratchBuffer.Length < num7)
			{
				scratchBuffer = new byte[Math.Max(998, num7)];
			}
			int num8 = ByteString.StringToBytes(this.paramName, scratchBuffer, 0, false);
			stream.Write(scratchBuffer, 0, num8);
			num += (long)num8;
			currentLineLength.IncrementBy(this.paramName.Length, num8);
			if (mimeString.Length != 0)
			{
				stream.Write(MimeString.Asterisk, 0, MimeString.Asterisk.Length);
				num += (long)MimeString.Asterisk.Length;
				currentLineLength.IncrementBy(MimeString.Asterisk.Length);
				mimeString.WriteTo(stream);
				num += (long)mimeString.Length;
				currentLineLength.IncrementBy(mimeString.Length);
			}
			if (this.valueEncoded)
			{
				stream.Write(MimeString.Asterisk, 0, MimeString.Asterisk.Length);
				num += (long)MimeString.Asterisk.Length;
				currentLineLength.IncrementBy(MimeString.Asterisk.Length);
			}
			stream.Write(MimeString.EqualTo, 0, MimeString.EqualTo.Length);
			num += (long)MimeString.EqualTo.Length;
			currentLineLength.IncrementBy(MimeString.EqualTo.Length);
			int num9 = 0;
			if (base.NextSibling != null)
			{
				num9++;
			}
			num += Header.QuoteAndFold(stream, fragments, 4026531839U, flag, false, encodingOptions.AllowUTF8, num9, ref currentLineLength, ref scratchBuffer);
			int num10 = 0;
			while (mimeNode != null)
			{
				MimeParameter mimeParameter = mimeNode as MimeParameter;
				if (mimeParameter != null)
				{
					num10++;
					mimeParameter.segmentNumber = num10;
					stream.Write(MimeString.Semicolon, 0, MimeString.Semicolon.Length);
					num += (long)MimeString.Semicolon.Length;
					currentLineLength.IncrementBy(MimeString.Semicolon.Length);
					num += Header.WriteLineEnd(stream, ref currentLineLength);
					stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
					num += (long)Header.LineStartWhitespace.Length;
					currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
					num += mimeNode.WriteTo(stream, encodingOptions, null, ref currentLineLength, ref scratchBuffer);
				}
				mimeNode = mimeNode.NextSibling;
			}
			return num;
		}

		internal override MimeNode ParseNextChild()
		{
			if (this.valueFragments.Length != 0 || this.decodedValue == null || this.decodedValue.Length == 0)
			{
				return null;
			}
			MimeNode internalLastChild = base.InternalLastChild;
			if (internalLastChild != null)
			{
				return null;
			}
			this.valueFragments = this.EncodeValue(this.decodedValue, base.GetDocumentEncodingOptions());
			return base.FirstChild;
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			if (this.allowAppend)
			{
				return refChild;
			}
			throw new NotSupportedException(Strings.ParametersCannotHaveChildNodes);
		}

		internal void AppendValue(ref MimeStringList list)
		{
			this.valueFragments.TakeOverAppend(ref list);
		}

		private static byte DecodeHexadecimal(byte ch)
		{
			ch -= 48;
			if (ch <= 9)
			{
				return ch;
			}
			ch -= 17;
			if (ch <= 5)
			{
				return ch + 10;
			}
			ch -= 32;
			if (ch <= 5)
			{
				return ch + 10;
			}
			return byte.MaxValue;
		}

		private bool IsQuotingReqired()
		{
			return this.paramName == "boundary" || this.paramName == "filename" || this.paramName == "name" || this.paramName == "id" || this.paramName == "charset";
		}

		private MimeStringList EncodeValue(string value, EncodingOptions encodingOptions)
		{
			this.valueFragments.Reset();
			int count = this.valueFragments.Count;
			if ((byte)(EncodingFlags.EnableRfc2231 & encodingOptions.EncodingFlags) != 0)
			{
				this.EncodeRfc2231(encodingOptions);
				return this.valueFragments;
			}
			return MimeCommon.EncodeValue(value, encodingOptions, ValueEncodingStyle.Normal);
		}

		private void EncodeRfc2231(EncodingOptions encodingOptions)
		{
			if (!MimeCommon.IsEncodingRequired(this.decodedValue, encodingOptions.AllowUTF8))
			{
				this.valueEncoded = false;
				this.segmentNumber = -1;
				this.valueFragments.AppendFragment(new MimeString(this.decodedValue));
				return;
			}
			Charset encodingCharset = encodingOptions.GetEncodingCharset();
			Encoding encoding = encodingCharset.GetEncoding();
			int num = 0;
			int num2 = 0;
			string text = (encodingOptions.CultureName == null) ? string.Empty : encodingOptions.CultureName;
			if ((encodingOptions.AllowUTF8 && encodingCharset.CodePage != 20127 && encodingCharset.CodePage != 65001) || (!encodingOptions.AllowUTF8 && encodingCharset.CodePage != 20127) || "en-us" != text)
			{
				string name = encodingCharset.Name;
				byte[] array = new byte[name.Length + text.Length + 2];
				num += ByteString.StringToBytes(name, array, num, false);
				num2 += name.Length;
				array[num++] = 39;
				num2++;
				num += ByteString.StringToBytes(text, array, num, false);
				num2 += text.Length;
				array[num++] = 39;
				num2++;
				this.valueFragments.AppendFragment(new MimeString(array, 0, num));
				this.valueEncoded = true;
			}
			int num3 = 78 - this.paramName.Length - 6;
			int num4 = 2;
			byte[] bytes = encoding.GetBytes(this.decodedValue);
			int i = this.EncodeRfc2231Segment(bytes, 0, num3 - num4 - num2, encodingOptions);
			this.segmentNumber = ((i < bytes.Length) ? 0 : -1);
			int num5 = 1;
			int num6 = 10;
			this.AllowAppend = true;
			while (i < bytes.Length)
			{
				MimeParameter mimeParameter = new MimeParameter(this.paramName);
				if (num6 == num5)
				{
					num4++;
					num6 *= 10;
				}
				i = mimeParameter.EncodeRfc2231Segment(bytes, i, num3 - num4, encodingOptions);
				mimeParameter.segmentNumber = num5++;
				base.InternalAppendChild(mimeParameter);
				if (10000 == num5)
				{
					break;
				}
			}
			this.AllowAppend = false;
		}

		private int EncodeRfc2231Segment(byte[] source, int sourceIndex, int maxValueLength, EncodingOptions encodingOptions)
		{
			byte[] array = new byte[maxValueLength * 4];
			int count = 0;
			int num = 0;
			while (sourceIndex < source.Length)
			{
				int i = 1;
				byte b = source[sourceIndex];
				bool flag = true;
				if (b >= 128)
				{
					if (encodingOptions.AllowUTF8 && MimeScan.IsUTF8NonASCII(source, sourceIndex, source.Length, out i))
					{
						flag = false;
					}
				}
				else if (!MimeScan.IsSegmentEncodingRequired(b))
				{
					flag = false;
				}
				if (flag)
				{
					if (num + 3 > maxValueLength)
					{
						break;
					}
					array[count++] = 37;
					array[count++] = MimeParameter.OctetEncoderMap[b >> 4];
					array[count++] = MimeParameter.OctetEncoderMap[(int)(b & 15)];
					num += 3;
					this.valueEncoded = true;
					sourceIndex++;
				}
				else
				{
					if (num + 1 > maxValueLength)
					{
						break;
					}
					while (i > 0)
					{
						array[count++] = source[sourceIndex++];
						i--;
					}
					num++;
				}
			}
			this.valueFragments.AppendFragment(new MimeString(array, 0, count));
			return sourceIndex;
		}

		private bool TryDecodeRfc2231(ref DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
		{
			decodingResults = default(DecodingResults);
			decodingResults.EncodingScheme = EncodingScheme.Rfc2231;
			Charset charset = null;
			byte[] sz = this.valueFragments.GetSz(4026531839U);
			int num = 0;
			if (this.valueEncoded)
			{
				int num2 = (sz == null) ? -1 : ByteString.IndexOf(sz, 39, 0, sz.Length);
				if (-1 < num2 && num2 < sz.Length - 1)
				{
					int num3 = ByteString.IndexOf(sz, 39, ++num2, sz.Length - num2);
					if (-1 < num3)
					{
						decodingResults.CharsetName = ByteString.BytesToString(sz, 0, num2 - 1, false);
						decodingResults.CultureName = ByteString.BytesToString(sz, num2, num3 - num2, false);
						if (!Charset.TryGetCharset(decodingResults.CharsetName, out charset))
						{
							decodingResults.DecodingFailed = true;
							return this.FallBackIfRequired(sz, decodingOptions, out value);
						}
						num = num3 + 1;
					}
				}
			}
			if (charset == null)
			{
				charset = decodingOptions.Charset;
				if (charset == null)
				{
					charset = DecodingOptions.DefaultCharset;
				}
			}
			decodingResults.CharsetName = charset.Name;
			Encoding encoding;
			if (!charset.TryGetEncoding(out encoding))
			{
				decodingResults.DecodingFailed = true;
				return this.FallBackIfRequired(sz, decodingOptions, out value);
			}
			int num4 = this.valueFragments.Length - num;
			for (MimeNode mimeNode = base.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
			{
				MimeParameter mimeParameter = mimeNode as MimeParameter;
				if (mimeParameter != null)
				{
					num4 += mimeParameter.RawLength;
				}
			}
			byte[] array = new byte[num4];
			int num5 = 0;
			if (sz != null && num < sz.Length)
			{
				num5 += this.DecodeRfc2231Octets(this.valueEncoded, sz, num, array, 0);
			}
			for (MimeNode mimeNode = base.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
			{
				MimeParameter mimeParameter2 = mimeNode as MimeParameter;
				if (mimeParameter2 != null)
				{
					byte[] rawValue = mimeParameter2.RawValue;
					if (rawValue != null)
					{
						num5 += this.DecodeRfc2231Octets(mimeParameter2.valueEncoded, mimeParameter2.RawValue, 0, array, num5);
					}
				}
			}
			value = ((num5 != 0) ? encoding.GetString(array, 0, num5) : string.Empty);
			if ((DecodingFlags.AllowControlCharacters & decodingOptions.DecodingFlags) == DecodingFlags.None)
			{
				value = MimeParameter.CorrectValue(value);
			}
			return true;
		}

		private int DecodeRfc2231Octets(bool decode, byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
		{
			if (decode)
			{
				int num = destinationIndex;
				while (sourceIndex < source.Length)
				{
					if (37 == source[sourceIndex] && sourceIndex + 2 < source.Length)
					{
						byte b = MimeParameter.DecodeHexadecimal(source[sourceIndex + 1]);
						if (255 != b)
						{
							byte b2 = MimeParameter.DecodeHexadecimal(source[sourceIndex + 2]);
							if (255 != b2)
							{
								sourceIndex += 3;
								destination[destinationIndex++] = (byte)(((int)b << 4) + (int)b2);
								continue;
							}
						}
					}
					destination[destinationIndex++] = source[sourceIndex++];
				}
				return destinationIndex - num;
			}
			int num2 = source.Length - sourceIndex;
			Buffer.BlockCopy(source, sourceIndex, destination, destinationIndex, num2);
			return num2;
		}

		internal const bool AllowUTF8Name = false;

		private const string DefaultLanguage = "en-us";

		private static readonly EncodingOptions EncodingOptionsAscii = new EncodingOptions(Charset.ASCII);

		private static readonly byte[] OctetEncoderMap = new byte[]
		{
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			65,
			66,
			67,
			68,
			69,
			70
		};

		private static readonly char[] ControlCharacters = new char[]
		{
			'\0',
			'\r',
			'\n',
			'\f',
			'\v'
		};

		private bool valueEncoded;

		private int segmentNumber = -1;

		private string paramName;

		private MimeStringList valueFragments = default(MimeStringList);

		private string decodedValue;

		private bool allowAppend;
	}
}
