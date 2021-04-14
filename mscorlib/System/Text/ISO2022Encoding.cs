using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;

namespace System.Text
{
	[Serializable]
	internal class ISO2022Encoding : DBCSCodePageEncoding
	{
		[SecurityCritical]
		internal ISO2022Encoding(int codePage) : base(codePage, ISO2022Encoding.tableBaseCodePages[codePage % 10])
		{
			this.m_bUseMlangTypeForSerialization = true;
		}

		[SecurityCritical]
		internal ISO2022Encoding(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			throw new ArgumentException(Environment.GetResourceString("Arg_ExecutionEngineException"));
		}

		[SecurityCritical]
		protected unsafe override string GetMemorySectionName()
		{
			int num = this.bFlagDataTable ? this.dataTableCodePage : this.CodePage;
			int codePage = this.CodePage;
			string format;
			if (codePage - 50220 > 2)
			{
				if (codePage != 50225)
				{
					if (codePage != 52936)
					{
						format = "CodePage_{0}_{1}_{2}_{3}_{4}";
					}
					else
					{
						format = "CodePage_{0}_{1}_{2}_{3}_{4}_HZ";
					}
				}
				else
				{
					format = "CodePage_{0}_{1}_{2}_{3}_{4}_ISO2022KR";
				}
			}
			else
			{
				format = "CodePage_{0}_{1}_{2}_{3}_{4}_ISO2022JP";
			}
			return string.Format(CultureInfo.InvariantCulture, format, new object[]
			{
				num,
				this.pCodePage->VersionMajor,
				this.pCodePage->VersionMinor,
				this.pCodePage->VersionRevision,
				this.pCodePage->VersionBuild
			});
		}

		protected override bool CleanUpBytes(ref int bytes)
		{
			int codePage = this.CodePage;
			if (codePage - 50220 > 2)
			{
				if (codePage != 50225)
				{
					if (codePage == 52936)
					{
						if (bytes >= 129 && bytes <= 254)
						{
							return false;
						}
					}
				}
				else
				{
					if (bytes >= 128 && bytes <= 255)
					{
						return false;
					}
					if (bytes >= 256 && ((bytes & 255) < 161 || (bytes & 255) == 255 || (bytes & 65280) < 41216 || (bytes & 65280) == 65280))
					{
						return false;
					}
					bytes &= 32639;
				}
			}
			else if (bytes >= 256)
			{
				if (bytes >= 64064 && bytes <= 64587)
				{
					if (bytes >= 64064 && bytes <= 64091)
					{
						if (bytes <= 64073)
						{
							bytes -= 2897;
						}
						else if (bytes >= 64074 && bytes <= 64083)
						{
							bytes -= 29430;
						}
						else if (bytes >= 64084 && bytes <= 64087)
						{
							bytes -= 2907;
						}
						else if (bytes == 64088)
						{
							bytes = 34698;
						}
						else if (bytes == 64089)
						{
							bytes = 34690;
						}
						else if (bytes == 64090)
						{
							bytes = 34692;
						}
						else if (bytes == 64091)
						{
							bytes = 34714;
						}
					}
					else if (bytes >= 64092 && bytes <= 64587)
					{
						byte b = (byte)bytes;
						if (b < 92)
						{
							bytes -= 3423;
						}
						else if (b >= 128 && b <= 155)
						{
							bytes -= 3357;
						}
						else
						{
							bytes -= 3356;
						}
					}
				}
				byte b2 = (byte)(bytes >> 8);
				byte b3 = (byte)bytes;
				b2 -= ((b2 > 159) ? 177 : 113);
				b2 = (byte)(((int)b2 << 1) + 1);
				if (b3 > 158)
				{
					b3 -= 126;
					b2 += 1;
				}
				else
				{
					if (b3 > 126)
					{
						b3 -= 1;
					}
					b3 -= 31;
				}
				bytes = ((int)b2 << 8 | (int)b3);
			}
			else
			{
				if (bytes >= 161 && bytes <= 223)
				{
					bytes += 3968;
				}
				if (bytes >= 129 && (bytes <= 159 || (bytes >= 224 && bytes <= 252)))
				{
					return false;
				}
			}
			return true;
		}

		[SecurityCritical]
		internal unsafe override int GetByteCount(char* chars, int count, EncoderNLS baseEncoder)
		{
			return this.GetBytes(chars, count, null, 0, baseEncoder);
		}

		[SecurityCritical]
		internal unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, EncoderNLS baseEncoder)
		{
			ISO2022Encoding.ISO2022Encoder encoder = (ISO2022Encoding.ISO2022Encoder)baseEncoder;
			int result = 0;
			int codePage = this.CodePage;
			if (codePage - 50220 > 2)
			{
				if (codePage != 50225)
				{
					if (codePage == 52936)
					{
						result = this.GetBytesCP52936(chars, charCount, bytes, byteCount, encoder);
					}
				}
				else
				{
					result = this.GetBytesCP50225KR(chars, charCount, bytes, byteCount, encoder);
				}
			}
			else
			{
				result = this.GetBytesCP5022xJP(chars, charCount, bytes, byteCount, encoder);
			}
			return result;
		}

		[SecurityCritical]
		internal unsafe override int GetCharCount(byte* bytes, int count, DecoderNLS baseDecoder)
		{
			return this.GetChars(bytes, count, null, 0, baseDecoder);
		}

		[SecurityCritical]
		internal unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount, DecoderNLS baseDecoder)
		{
			ISO2022Encoding.ISO2022Decoder decoder = (ISO2022Encoding.ISO2022Decoder)baseDecoder;
			int result = 0;
			int codePage = this.CodePage;
			if (codePage - 50220 > 2)
			{
				if (codePage != 50225)
				{
					if (codePage == 52936)
					{
						result = this.GetCharsCP52936(bytes, byteCount, chars, charCount, decoder);
					}
				}
				else
				{
					result = this.GetCharsCP50225KR(bytes, byteCount, chars, charCount, decoder);
				}
			}
			else
			{
				result = this.GetCharsCP5022xJP(bytes, byteCount, chars, charCount, decoder);
			}
			return result;
		}

		[SecurityCritical]
		private unsafe int GetBytesCP5022xJP(char* chars, int charCount, byte* bytes, int byteCount, ISO2022Encoding.ISO2022Encoder encoder)
		{
			Encoding.EncodingByteBuffer encodingByteBuffer = new Encoding.EncodingByteBuffer(this, encoder, bytes, byteCount, chars, charCount);
			ISO2022Encoding.ISO2022Modes iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
			ISO2022Encoding.ISO2022Modes iso2022Modes2 = ISO2022Encoding.ISO2022Modes.ModeASCII;
			if (encoder != null)
			{
				char charLeftOver = encoder.charLeftOver;
				iso2022Modes = encoder.currentMode;
				iso2022Modes2 = encoder.shiftInOutMode;
				if (charLeftOver > '\0')
				{
					encodingByteBuffer.Fallback(charLeftOver);
				}
			}
			while (encodingByteBuffer.MoreData)
			{
				char nextChar = encodingByteBuffer.GetNextChar();
				ushort num = this.mapUnicodeToBytes[(IntPtr)nextChar];
				byte b;
				byte b2;
				for (;;)
				{
					b = (byte)(num >> 8);
					b2 = (byte)(num & 255);
					if (b != 16)
					{
						goto IL_10A;
					}
					if (this.CodePage != 50220)
					{
						goto IL_BE;
					}
					if (b2 < 33 || (int)b2 >= 33 + ISO2022Encoding.HalfToFullWidthKanaTable.Length)
					{
						break;
					}
					num = (ISO2022Encoding.HalfToFullWidthKanaTable[(int)(b2 - 33)] & 32639);
				}
				encodingByteBuffer.Fallback(nextChar);
				continue;
				IL_BE:
				if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana)
				{
					if (this.CodePage == 50222)
					{
						if (!encodingByteBuffer.AddByte(14))
						{
							break;
						}
						iso2022Modes2 = iso2022Modes;
						iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana;
					}
					else
					{
						if (!encodingByteBuffer.AddByte(27, 40, 73))
						{
							break;
						}
						iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana;
					}
				}
				if (!encodingByteBuffer.AddByte(b2 & 127))
				{
					break;
				}
				continue;
				IL_10A:
				if (b != 0)
				{
					if (this.CodePage == 50222 && iso2022Modes == ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana)
					{
						if (!encodingByteBuffer.AddByte(15))
						{
							break;
						}
						iso2022Modes = iso2022Modes2;
					}
					if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeJIS0208)
					{
						if (!encodingByteBuffer.AddByte(27, 36, 66))
						{
							break;
						}
						iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeJIS0208;
					}
					if (!encodingByteBuffer.AddByte(b, b2))
					{
						break;
					}
				}
				else if (num != 0 || nextChar == '\0')
				{
					if (this.CodePage == 50222 && iso2022Modes == ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana)
					{
						if (!encodingByteBuffer.AddByte(15))
						{
							break;
						}
						iso2022Modes = iso2022Modes2;
					}
					if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeASCII)
					{
						if (!encodingByteBuffer.AddByte(27, 40, 66))
						{
							break;
						}
						iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
					}
					if (!encodingByteBuffer.AddByte(b2))
					{
						break;
					}
				}
				else
				{
					encodingByteBuffer.Fallback(nextChar);
				}
			}
			if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeASCII && (encoder == null || encoder.MustFlush))
			{
				if (this.CodePage == 50222 && iso2022Modes == ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana)
				{
					if (encodingByteBuffer.AddByte(15))
					{
						iso2022Modes = iso2022Modes2;
					}
					else
					{
						encodingByteBuffer.GetNextChar();
					}
				}
				if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeASCII && (this.CodePage != 50222 || iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana))
				{
					if (encodingByteBuffer.AddByte(27, 40, 66))
					{
						iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
					}
					else
					{
						encodingByteBuffer.GetNextChar();
					}
				}
			}
			if (bytes != null && encoder != null)
			{
				encoder.currentMode = iso2022Modes;
				encoder.shiftInOutMode = iso2022Modes2;
				if (!encodingByteBuffer.fallbackBuffer.bUsedEncoder)
				{
					encoder.charLeftOver = '\0';
				}
				encoder.m_charsUsed = encodingByteBuffer.CharsUsed;
			}
			return encodingByteBuffer.Count;
		}

		[SecurityCritical]
		private unsafe int GetBytesCP50225KR(char* chars, int charCount, byte* bytes, int byteCount, ISO2022Encoding.ISO2022Encoder encoder)
		{
			Encoding.EncodingByteBuffer encodingByteBuffer = new Encoding.EncodingByteBuffer(this, encoder, bytes, byteCount, chars, charCount);
			ISO2022Encoding.ISO2022Modes iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
			ISO2022Encoding.ISO2022Modes iso2022Modes2 = ISO2022Encoding.ISO2022Modes.ModeASCII;
			if (encoder != null)
			{
				char charLeftOver = encoder.charLeftOver;
				iso2022Modes = encoder.currentMode;
				iso2022Modes2 = encoder.shiftInOutMode;
				if (charLeftOver > '\0')
				{
					encodingByteBuffer.Fallback(charLeftOver);
				}
			}
			while (encodingByteBuffer.MoreData)
			{
				char nextChar = encodingByteBuffer.GetNextChar();
				ushort num = this.mapUnicodeToBytes[(IntPtr)nextChar];
				byte b = (byte)(num >> 8);
				byte b2 = (byte)(num & 255);
				if (b != 0)
				{
					if (iso2022Modes2 != ISO2022Encoding.ISO2022Modes.ModeKR)
					{
						if (!encodingByteBuffer.AddByte(27, 36, 41, 67))
						{
							break;
						}
						iso2022Modes2 = ISO2022Encoding.ISO2022Modes.ModeKR;
					}
					if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeKR)
					{
						if (!encodingByteBuffer.AddByte(14))
						{
							break;
						}
						iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeKR;
					}
					if (!encodingByteBuffer.AddByte(b, b2))
					{
						break;
					}
				}
				else if (num != 0 || nextChar == '\0')
				{
					if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeASCII)
					{
						if (!encodingByteBuffer.AddByte(15))
						{
							break;
						}
						iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
					}
					if (!encodingByteBuffer.AddByte(b2))
					{
						break;
					}
				}
				else
				{
					encodingByteBuffer.Fallback(nextChar);
				}
			}
			if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeASCII && (encoder == null || encoder.MustFlush))
			{
				if (encodingByteBuffer.AddByte(15))
				{
					iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
				else
				{
					encodingByteBuffer.GetNextChar();
				}
			}
			if (bytes != null && encoder != null)
			{
				if (!encodingByteBuffer.fallbackBuffer.bUsedEncoder)
				{
					encoder.charLeftOver = '\0';
				}
				encoder.currentMode = iso2022Modes;
				if (!encoder.MustFlush || encoder.charLeftOver != '\0')
				{
					encoder.shiftInOutMode = iso2022Modes2;
				}
				else
				{
					encoder.shiftInOutMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
				encoder.m_charsUsed = encodingByteBuffer.CharsUsed;
			}
			return encodingByteBuffer.Count;
		}

		[SecurityCritical]
		private unsafe int GetBytesCP52936(char* chars, int charCount, byte* bytes, int byteCount, ISO2022Encoding.ISO2022Encoder encoder)
		{
			Encoding.EncodingByteBuffer encodingByteBuffer = new Encoding.EncodingByteBuffer(this, encoder, bytes, byteCount, chars, charCount);
			ISO2022Encoding.ISO2022Modes iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
			if (encoder != null)
			{
				char charLeftOver = encoder.charLeftOver;
				iso2022Modes = encoder.currentMode;
				if (charLeftOver > '\0')
				{
					encodingByteBuffer.Fallback(charLeftOver);
				}
			}
			while (encodingByteBuffer.MoreData)
			{
				char nextChar = encodingByteBuffer.GetNextChar();
				ushort num = this.mapUnicodeToBytes[(IntPtr)nextChar];
				if (num == 0 && nextChar != '\0')
				{
					encodingByteBuffer.Fallback(nextChar);
				}
				else
				{
					byte b = (byte)(num >> 8);
					byte b2 = (byte)(num & 255);
					if ((b != 0 && (b < 161 || b > 247 || b2 < 161 || b2 > 254)) || (b == 0 && b2 > 128 && b2 != 255))
					{
						encodingByteBuffer.Fallback(nextChar);
					}
					else if (b != 0)
					{
						if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeHZ)
						{
							if (!encodingByteBuffer.AddByte(126, 123, 2))
							{
								break;
							}
							iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeHZ;
						}
						if (!encodingByteBuffer.AddByte(b & 127, b2 & 127))
						{
							break;
						}
					}
					else
					{
						if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeASCII)
						{
							if (!encodingByteBuffer.AddByte(126, 125, (b2 == 126) ? 2 : 1))
							{
								break;
							}
							iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
						}
						if ((b2 == 126 && !encodingByteBuffer.AddByte(126, 1)) || !encodingByteBuffer.AddByte(b2))
						{
							break;
						}
					}
				}
			}
			if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeASCII && (encoder == null || encoder.MustFlush))
			{
				if (encodingByteBuffer.AddByte(126, 125))
				{
					iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
				else
				{
					encodingByteBuffer.GetNextChar();
				}
			}
			if (encoder != null && bytes != null)
			{
				encoder.currentMode = iso2022Modes;
				if (!encodingByteBuffer.fallbackBuffer.bUsedEncoder)
				{
					encoder.charLeftOver = '\0';
				}
				encoder.m_charsUsed = encodingByteBuffer.CharsUsed;
			}
			return encodingByteBuffer.Count;
		}

		[SecurityCritical]
		private unsafe int GetCharsCP5022xJP(byte* bytes, int byteCount, char* chars, int charCount, ISO2022Encoding.ISO2022Decoder decoder)
		{
			Encoding.EncodingCharBuffer encodingCharBuffer = new Encoding.EncodingCharBuffer(this, decoder, chars, charCount, bytes, byteCount);
			ISO2022Encoding.ISO2022Modes iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
			ISO2022Encoding.ISO2022Modes iso2022Modes2 = ISO2022Encoding.ISO2022Modes.ModeASCII;
			byte[] array = new byte[4];
			int num = 0;
			if (decoder != null)
			{
				iso2022Modes = decoder.currentMode;
				iso2022Modes2 = decoder.shiftInOutMode;
				num = decoder.bytesLeftOverCount;
				for (int i = 0; i < num; i++)
				{
					array[i] = decoder.bytesLeftOver[i];
				}
			}
			while (encodingCharBuffer.MoreData || num > 0)
			{
				byte b;
				if (num > 0)
				{
					if (array[0] == 27)
					{
						if (!encodingCharBuffer.MoreData)
						{
							if (decoder != null && !decoder.MustFlush)
							{
								break;
							}
						}
						else
						{
							array[num++] = encodingCharBuffer.GetNextByte();
							ISO2022Encoding.ISO2022Modes iso2022Modes3 = this.CheckEscapeSequenceJP(array, num);
							if (iso2022Modes3 != ISO2022Encoding.ISO2022Modes.ModeInvalidEscape)
							{
								if (iso2022Modes3 != ISO2022Encoding.ISO2022Modes.ModeIncompleteEscape)
								{
									num = 0;
									iso2022Modes2 = (iso2022Modes = iso2022Modes3);
									continue;
								}
								continue;
							}
						}
					}
					b = this.DecrementEscapeBytes(ref array, ref num);
				}
				else
				{
					b = encodingCharBuffer.GetNextByte();
					if (b == 27)
					{
						if (num == 0)
						{
							array[0] = b;
							num = 1;
							continue;
						}
						encodingCharBuffer.AdjustBytes(-1);
					}
				}
				if (b == 14)
				{
					iso2022Modes2 = iso2022Modes;
					iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana;
				}
				else if (b == 15)
				{
					iso2022Modes = iso2022Modes2;
				}
				else
				{
					ushort num2 = (ushort)b;
					bool flag = false;
					if (iso2022Modes == ISO2022Encoding.ISO2022Modes.ModeJIS0208)
					{
						if (num > 0)
						{
							if (array[0] != 27)
							{
								num2 = (ushort)(num2 << 8);
								num2 |= (ushort)this.DecrementEscapeBytes(ref array, ref num);
								flag = true;
							}
						}
						else if (encodingCharBuffer.MoreData)
						{
							num2 = (ushort)(num2 << 8);
							num2 |= (ushort)encodingCharBuffer.GetNextByte();
							flag = true;
						}
						else
						{
							if (decoder == null || decoder.MustFlush)
							{
								encodingCharBuffer.Fallback(b);
								break;
							}
							if (chars != null)
							{
								array[0] = b;
								num = 1;
								break;
							}
							break;
						}
						if (flag && (num2 & 65280) == 10752)
						{
							num2 &= 255;
							num2 |= 4096;
						}
					}
					else if (num2 >= 161 && num2 <= 223)
					{
						num2 |= 4096;
						num2 &= 65407;
					}
					else if (iso2022Modes == ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana)
					{
						num2 |= 4096;
					}
					char c = this.mapBytesToUnicode[num2];
					if (c == '\0' && num2 != 0)
					{
						if (flag)
						{
							if (!encodingCharBuffer.Fallback((byte)(num2 >> 8), (byte)num2))
							{
								break;
							}
						}
						else if (!encodingCharBuffer.Fallback(b))
						{
							break;
						}
					}
					else if (!encodingCharBuffer.AddChar(c, flag ? 2 : 1))
					{
						break;
					}
				}
			}
			if (chars != null && decoder != null)
			{
				if (!decoder.MustFlush || num != 0)
				{
					decoder.currentMode = iso2022Modes;
					decoder.shiftInOutMode = iso2022Modes2;
					decoder.bytesLeftOverCount = num;
					decoder.bytesLeftOver = array;
				}
				else
				{
					decoder.currentMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
					decoder.shiftInOutMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
					decoder.bytesLeftOverCount = 0;
				}
				decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
			}
			return encodingCharBuffer.Count;
		}

		private ISO2022Encoding.ISO2022Modes CheckEscapeSequenceJP(byte[] bytes, int escapeCount)
		{
			if (bytes[0] != 27)
			{
				return ISO2022Encoding.ISO2022Modes.ModeInvalidEscape;
			}
			if (escapeCount < 3)
			{
				return ISO2022Encoding.ISO2022Modes.ModeIncompleteEscape;
			}
			if (bytes[1] == 40)
			{
				if (bytes[2] == 66)
				{
					return ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
				if (bytes[2] == 72)
				{
					return ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
				if (bytes[2] == 74)
				{
					return ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
				if (bytes[2] == 73)
				{
					return ISO2022Encoding.ISO2022Modes.ModeHalfwidthKatakana;
				}
			}
			else if (bytes[1] == 36)
			{
				if (bytes[2] == 64 || bytes[2] == 66)
				{
					return ISO2022Encoding.ISO2022Modes.ModeJIS0208;
				}
				if (escapeCount < 4)
				{
					return ISO2022Encoding.ISO2022Modes.ModeIncompleteEscape;
				}
				if (bytes[2] == 40 && bytes[3] == 68)
				{
					return ISO2022Encoding.ISO2022Modes.ModeJIS0208;
				}
			}
			else if (bytes[1] == 38 && bytes[2] == 64)
			{
				return ISO2022Encoding.ISO2022Modes.ModeNOOP;
			}
			return ISO2022Encoding.ISO2022Modes.ModeInvalidEscape;
		}

		private byte DecrementEscapeBytes(ref byte[] bytes, ref int count)
		{
			count--;
			byte result = bytes[0];
			for (int i = 0; i < count; i++)
			{
				bytes[i] = bytes[i + 1];
			}
			bytes[count] = 0;
			return result;
		}

		[SecurityCritical]
		private unsafe int GetCharsCP50225KR(byte* bytes, int byteCount, char* chars, int charCount, ISO2022Encoding.ISO2022Decoder decoder)
		{
			Encoding.EncodingCharBuffer encodingCharBuffer = new Encoding.EncodingCharBuffer(this, decoder, chars, charCount, bytes, byteCount);
			ISO2022Encoding.ISO2022Modes iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
			byte[] array = new byte[4];
			int num = 0;
			if (decoder != null)
			{
				iso2022Modes = decoder.currentMode;
				num = decoder.bytesLeftOverCount;
				for (int i = 0; i < num; i++)
				{
					array[i] = decoder.bytesLeftOver[i];
				}
			}
			while (encodingCharBuffer.MoreData || num > 0)
			{
				byte b;
				if (num > 0)
				{
					if (array[0] == 27)
					{
						if (!encodingCharBuffer.MoreData)
						{
							if (decoder != null && !decoder.MustFlush)
							{
								break;
							}
						}
						else
						{
							array[num++] = encodingCharBuffer.GetNextByte();
							ISO2022Encoding.ISO2022Modes iso2022Modes2 = this.CheckEscapeSequenceKR(array, num);
							if (iso2022Modes2 != ISO2022Encoding.ISO2022Modes.ModeInvalidEscape)
							{
								if (iso2022Modes2 != ISO2022Encoding.ISO2022Modes.ModeIncompleteEscape)
								{
									num = 0;
									continue;
								}
								continue;
							}
						}
					}
					b = this.DecrementEscapeBytes(ref array, ref num);
				}
				else
				{
					b = encodingCharBuffer.GetNextByte();
					if (b == 27)
					{
						if (num == 0)
						{
							array[0] = b;
							num = 1;
							continue;
						}
						encodingCharBuffer.AdjustBytes(-1);
					}
				}
				if (b == 14)
				{
					iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeKR;
				}
				else if (b == 15)
				{
					iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
				else
				{
					ushort num2 = (ushort)b;
					bool flag = false;
					if (iso2022Modes == ISO2022Encoding.ISO2022Modes.ModeKR && b != 32 && b != 9 && b != 10)
					{
						if (num > 0)
						{
							if (array[0] != 27)
							{
								num2 = (ushort)(num2 << 8);
								num2 |= (ushort)this.DecrementEscapeBytes(ref array, ref num);
								flag = true;
							}
						}
						else if (encodingCharBuffer.MoreData)
						{
							num2 = (ushort)(num2 << 8);
							num2 |= (ushort)encodingCharBuffer.GetNextByte();
							flag = true;
						}
						else
						{
							if (decoder == null || decoder.MustFlush)
							{
								encodingCharBuffer.Fallback(b);
								break;
							}
							if (chars != null)
							{
								array[0] = b;
								num = 1;
								break;
							}
							break;
						}
					}
					char c = this.mapBytesToUnicode[num2];
					if (c == '\0' && num2 != 0)
					{
						if (flag)
						{
							if (!encodingCharBuffer.Fallback((byte)(num2 >> 8), (byte)num2))
							{
								break;
							}
						}
						else if (!encodingCharBuffer.Fallback(b))
						{
							break;
						}
					}
					else if (!encodingCharBuffer.AddChar(c, flag ? 2 : 1))
					{
						break;
					}
				}
			}
			if (chars != null && decoder != null)
			{
				if (!decoder.MustFlush || num != 0)
				{
					decoder.currentMode = iso2022Modes;
					decoder.bytesLeftOverCount = num;
					decoder.bytesLeftOver = array;
				}
				else
				{
					decoder.currentMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
					decoder.shiftInOutMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
					decoder.bytesLeftOverCount = 0;
				}
				decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
			}
			return encodingCharBuffer.Count;
		}

		private ISO2022Encoding.ISO2022Modes CheckEscapeSequenceKR(byte[] bytes, int escapeCount)
		{
			if (bytes[0] != 27)
			{
				return ISO2022Encoding.ISO2022Modes.ModeInvalidEscape;
			}
			if (escapeCount < 4)
			{
				return ISO2022Encoding.ISO2022Modes.ModeIncompleteEscape;
			}
			if (bytes[1] == 36 && bytes[2] == 41 && bytes[3] == 67)
			{
				return ISO2022Encoding.ISO2022Modes.ModeKR;
			}
			return ISO2022Encoding.ISO2022Modes.ModeInvalidEscape;
		}

		[SecurityCritical]
		private unsafe int GetCharsCP52936(byte* bytes, int byteCount, char* chars, int charCount, ISO2022Encoding.ISO2022Decoder decoder)
		{
			Encoding.EncodingCharBuffer encodingCharBuffer = new Encoding.EncodingCharBuffer(this, decoder, chars, charCount, bytes, byteCount);
			ISO2022Encoding.ISO2022Modes iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
			int num = -1;
			bool flag = false;
			if (decoder != null)
			{
				iso2022Modes = decoder.currentMode;
				if (decoder.bytesLeftOverCount != 0)
				{
					num = (int)decoder.bytesLeftOver[0];
				}
			}
			while (encodingCharBuffer.MoreData || num >= 0)
			{
				byte b;
				if (num >= 0)
				{
					b = (byte)num;
					num = -1;
				}
				else
				{
					b = encodingCharBuffer.GetNextByte();
				}
				if (b == 126)
				{
					if (!encodingCharBuffer.MoreData)
					{
						if (decoder == null || decoder.MustFlush)
						{
							encodingCharBuffer.Fallback(b);
							break;
						}
						if (decoder != null)
						{
							decoder.ClearMustFlush();
						}
						if (chars != null)
						{
							decoder.bytesLeftOverCount = 1;
							decoder.bytesLeftOver[0] = 126;
							flag = true;
							break;
						}
						break;
					}
					else
					{
						b = encodingCharBuffer.GetNextByte();
						if (b == 126 && iso2022Modes == ISO2022Encoding.ISO2022Modes.ModeASCII)
						{
							if (!encodingCharBuffer.AddChar((char)b, 2))
							{
								break;
							}
							continue;
						}
						else
						{
							if (b == 123)
							{
								iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeHZ;
								continue;
							}
							if (b == 125)
							{
								iso2022Modes = ISO2022Encoding.ISO2022Modes.ModeASCII;
								continue;
							}
							if (b == 10)
							{
								continue;
							}
							encodingCharBuffer.AdjustBytes(-1);
							b = 126;
						}
					}
				}
				if (iso2022Modes != ISO2022Encoding.ISO2022Modes.ModeASCII && b >= 32)
				{
					if (!encodingCharBuffer.MoreData)
					{
						if (decoder == null || decoder.MustFlush)
						{
							encodingCharBuffer.Fallback(b);
							break;
						}
						if (decoder != null)
						{
							decoder.ClearMustFlush();
						}
						if (chars != null)
						{
							decoder.bytesLeftOverCount = 1;
							decoder.bytesLeftOver[0] = b;
							flag = true;
							break;
						}
						break;
					}
					else
					{
						byte nextByte = encodingCharBuffer.GetNextByte();
						ushort num2 = (ushort)((int)b << 8 | (int)nextByte);
						char c;
						if (b == 32 && nextByte != 0)
						{
							c = (char)nextByte;
						}
						else
						{
							if ((b < 33 || b > 119 || nextByte < 33 || nextByte > 126) && (b < 161 || b > 247 || nextByte < 161 || nextByte > 254))
							{
								if (nextByte == 32 && 33 <= b && b <= 125)
								{
									num2 = 8481;
								}
								else
								{
									if (!encodingCharBuffer.Fallback((byte)(num2 >> 8), (byte)num2))
									{
										break;
									}
									continue;
								}
							}
							num2 |= 32896;
							c = this.mapBytesToUnicode[num2];
						}
						if (c == '\0' && num2 != 0)
						{
							if (!encodingCharBuffer.Fallback((byte)(num2 >> 8), (byte)num2))
							{
								break;
							}
						}
						else if (!encodingCharBuffer.AddChar(c, 2))
						{
							break;
						}
					}
				}
				else
				{
					char c2 = this.mapBytesToUnicode[b];
					if ((c2 == '\0' || c2 == '\0') && b != 0)
					{
						if (!encodingCharBuffer.Fallback(b))
						{
							break;
						}
					}
					else if (!encodingCharBuffer.AddChar(c2))
					{
						break;
					}
				}
			}
			if (chars != null && decoder != null)
			{
				if (!flag)
				{
					decoder.bytesLeftOverCount = 0;
				}
				if (decoder.MustFlush && decoder.bytesLeftOverCount == 0)
				{
					decoder.currentMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
				else
				{
					decoder.currentMode = iso2022Modes;
				}
				decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
			}
			return encodingCharBuffer.Count;
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			long num = (long)charCount + 1L;
			if (base.EncoderFallback.MaxCharCount > 1)
			{
				num *= (long)base.EncoderFallback.MaxCharCount;
			}
			int num2 = 2;
			int num3 = 0;
			int num4 = 0;
			int codePage = this.CodePage;
			switch (codePage)
			{
			case 50220:
			case 50221:
				num2 = 5;
				num4 = 3;
				break;
			case 50222:
				num2 = 5;
				num4 = 4;
				break;
			case 50223:
			case 50224:
				break;
			case 50225:
				num2 = 3;
				num3 = 4;
				num4 = 1;
				break;
			default:
				if (codePage == 52936)
				{
					num2 = 4;
					num4 = 2;
				}
				break;
			}
			num *= (long)num2;
			num += (long)(num3 + num4);
			if (num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("charCount", Environment.GetResourceString("ArgumentOutOfRange_GetByteCountOverflow"));
			}
			return (int)num;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			int num = 1;
			int num2 = 1;
			int codePage = this.CodePage;
			if (codePage - 50220 > 2 && codePage != 50225)
			{
				if (codePage == 52936)
				{
					num = 1;
					num2 = 1;
				}
			}
			else
			{
				num = 1;
				num2 = 3;
			}
			long num3 = (long)byteCount * (long)num + (long)num2;
			if (base.DecoderFallback.MaxCharCount > 1)
			{
				num3 *= (long)base.DecoderFallback.MaxCharCount;
			}
			if (num3 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("byteCount", Environment.GetResourceString("ArgumentOutOfRange_GetCharCountOverflow"));
			}
			return (int)num3;
		}

		public override Encoder GetEncoder()
		{
			return new ISO2022Encoding.ISO2022Encoder(this);
		}

		public override Decoder GetDecoder()
		{
			return new ISO2022Encoding.ISO2022Decoder(this);
		}

		private const byte SHIFT_OUT = 14;

		private const byte SHIFT_IN = 15;

		private const byte ESCAPE = 27;

		private const byte LEADBYTE_HALFWIDTH = 16;

		private static int[] tableBaseCodePages = new int[]
		{
			932,
			932,
			932,
			0,
			0,
			949,
			936,
			0,
			0,
			0,
			0,
			0
		};

		private static ushort[] HalfToFullWidthKanaTable = new ushort[]
		{
			41379,
			41430,
			41431,
			41378,
			41382,
			42482,
			42401,
			42403,
			42405,
			42407,
			42409,
			42467,
			42469,
			42471,
			42435,
			41404,
			42402,
			42404,
			42406,
			42408,
			42410,
			42411,
			42413,
			42415,
			42417,
			42419,
			42421,
			42423,
			42425,
			42427,
			42429,
			42431,
			42433,
			42436,
			42438,
			42440,
			42442,
			42443,
			42444,
			42445,
			42446,
			42447,
			42450,
			42453,
			42456,
			42459,
			42462,
			42463,
			42464,
			42465,
			42466,
			42468,
			42470,
			42472,
			42473,
			42474,
			42475,
			42476,
			42477,
			42479,
			42483,
			41387,
			41388
		};

		internal enum ISO2022Modes
		{
			ModeHalfwidthKatakana,
			ModeJIS0208,
			ModeKR = 5,
			ModeHZ,
			ModeGB2312,
			ModeCNS11643_1 = 9,
			ModeCNS11643_2,
			ModeASCII,
			ModeIncompleteEscape = -1,
			ModeInvalidEscape = -2,
			ModeNOOP = -3
		}

		[Serializable]
		internal class ISO2022Encoder : EncoderNLS
		{
			internal ISO2022Encoder(EncodingNLS encoding) : base(encoding)
			{
			}

			public override void Reset()
			{
				this.currentMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
				this.shiftInOutMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
				this.charLeftOver = '\0';
				if (this.m_fallbackBuffer != null)
				{
					this.m_fallbackBuffer.Reset();
				}
			}

			internal override bool HasState
			{
				get
				{
					return this.charLeftOver != '\0' || this.currentMode != ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
			}

			internal ISO2022Encoding.ISO2022Modes currentMode;

			internal ISO2022Encoding.ISO2022Modes shiftInOutMode;
		}

		[Serializable]
		internal class ISO2022Decoder : DecoderNLS
		{
			internal ISO2022Decoder(EncodingNLS encoding) : base(encoding)
			{
			}

			public override void Reset()
			{
				this.bytesLeftOverCount = 0;
				this.bytesLeftOver = new byte[4];
				this.currentMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
				this.shiftInOutMode = ISO2022Encoding.ISO2022Modes.ModeASCII;
				if (this.m_fallbackBuffer != null)
				{
					this.m_fallbackBuffer.Reset();
				}
			}

			internal override bool HasState
			{
				get
				{
					return this.bytesLeftOverCount != 0 || this.currentMode != ISO2022Encoding.ISO2022Modes.ModeASCII;
				}
			}

			internal byte[] bytesLeftOver;

			internal int bytesLeftOverCount;

			internal ISO2022Encoding.ISO2022Modes currentMode;

			internal ISO2022Encoding.ISO2022Modes shiftInOutMode;
		}
	}
}
