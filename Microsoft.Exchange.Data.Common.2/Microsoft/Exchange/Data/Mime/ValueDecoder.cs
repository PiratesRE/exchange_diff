using System;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.Mime
{
	internal struct ValueDecoder
	{
		public ValueDecoder(MimeStringList lines, uint linesMask)
		{
			this.iterator = new ValueIterator(lines, linesMask);
			this.maxCharsetNameLength = Math.Max(60, Charset.MaxCharsetNameLength) + 10 + 1;
		}

		public bool TryDecodeValue(Charset defaultCharset, bool enableFallback, bool allowControlCharacters, bool enable2047, bool enableJisDetection, bool enableUtf8Detection, bool enableDbcsDetection, out string charsetName, out string cultureName, out EncodingScheme encodingScheme, out string value)
		{
			charsetName = null;
			cultureName = null;
			encodingScheme = EncodingScheme.None;
			value = null;
			StringBuilder stringBuilder = ScratchPad.GetStringBuilder(Math.Min(1024, this.iterator.TotalLength));
			char[] array = null;
			byte[] array2 = null;
			ValuePosition currentPosition = this.iterator.CurrentPosition;
			bool flag = false;
			bool flag2 = false;
			if (defaultCharset != null && (enableJisDetection || enableUtf8Detection || (enableDbcsDetection && FeInboundCharsetDetector.IsSupportedFarEastCharset(defaultCharset))))
			{
				defaultCharset = this.DetectValueCharset(defaultCharset, enableJisDetection, enableUtf8Detection, enableDbcsDetection, out encodingScheme);
				flag2 = true;
			}
			Decoder decoder = null;
			string text = null;
			if (!enable2047)
			{
				this.iterator.SkipToEof();
			}
			else
			{
				string lastEncodedWordLanguage = null;
				Charset charset = null;
				Decoder decoder2 = null;
				bool flag3 = true;
				string text2;
				for (;;)
				{
					this.ParseRawFragment(ref flag);
					if (this.iterator.Eof)
					{
						goto IL_217;
					}
					ValuePosition currentPosition2 = this.iterator.CurrentPosition;
					string text3;
					byte bOrQ;
					ValuePosition encodedWordContentStart;
					ValuePosition encodedWordContentEnd;
					if (!this.ParseEncodedWord(text, lastEncodedWordLanguage, ref array2, out text2, out text3, out bOrQ, out encodedWordContentStart, out encodedWordContentEnd))
					{
						flag = false;
					}
					else
					{
						if (text == null)
						{
							encodingScheme = EncodingScheme.Rfc2047;
							charsetName = text2;
							cultureName = text3;
						}
						text = text2;
						if (currentPosition != currentPosition2 && !flag)
						{
							if (!flag3)
							{
								this.FlushDecoder(decoder2, allowControlCharacters, ref array2, ref array, stringBuilder);
								flag3 = true;
							}
							if (decoder == null)
							{
								if (defaultCharset == null || !defaultCharset.IsAvailable)
								{
									if (!enableFallback)
									{
										break;
									}
								}
								else
								{
									decoder = defaultCharset.GetEncoding().GetDecoder();
								}
							}
							if (decoder != null)
							{
								this.ConvertRawFragment(currentPosition, currentPosition2, decoder, allowControlCharacters, ref array, stringBuilder);
							}
							else
							{
								this.ZeroExpandFragment(currentPosition, currentPosition2, allowControlCharacters, stringBuilder);
							}
						}
						Charset charset2;
						if (!Charset.TryGetCharset(text2, out charset2))
						{
							if (!flag3)
							{
								this.FlushDecoder(decoder2, allowControlCharacters, ref array2, ref array, stringBuilder);
								flag3 = true;
							}
							if (!enableFallback)
							{
								goto Block_19;
							}
							decoder2 = null;
						}
						else if (charset2 != charset)
						{
							if (!flag3)
							{
								this.FlushDecoder(decoder2, allowControlCharacters, ref array2, ref array, stringBuilder);
								flag3 = true;
							}
							if (!charset2.IsAvailable)
							{
								if (!enableFallback)
								{
									goto Block_23;
								}
								decoder2 = null;
							}
							else
							{
								decoder2 = charset2.GetEncoding().GetDecoder();
								charset = charset2;
							}
						}
						if (decoder2 != null)
						{
							this.DecodeEncodedWord(bOrQ, decoder2, encodedWordContentStart, encodedWordContentEnd, allowControlCharacters, ref array2, ref array, stringBuilder);
							flag3 = false;
						}
						else
						{
							this.ZeroExpandFragment(currentPosition2, this.iterator.CurrentPosition, allowControlCharacters, stringBuilder);
						}
						currentPosition = this.iterator.CurrentPosition;
						flag = true;
					}
				}
				charsetName = ((defaultCharset == null) ? null : defaultCharset.Name);
				return false;
				Block_19:
				charsetName = text2;
				return false;
				Block_23:
				charsetName = text2;
				return false;
				IL_217:
				if (!flag3)
				{
					this.FlushDecoder(decoder2, allowControlCharacters, ref array2, ref array, stringBuilder);
				}
			}
			if (currentPosition != this.iterator.CurrentPosition)
			{
				if (text == null)
				{
					if (flag2 && encodingScheme == EncodingScheme.None && defaultCharset != null && !defaultCharset.IsSevenBit && defaultCharset.AsciiSupport == CodePageAsciiSupport.Complete)
					{
						charsetName = Charset.ASCII.Name;
					}
					else
					{
						charsetName = ((defaultCharset == null) ? null : defaultCharset.Name);
					}
				}
				if (decoder == null)
				{
					if (defaultCharset == null || !defaultCharset.IsAvailable)
					{
						if (!enableFallback)
						{
							charsetName = ((defaultCharset == null) ? null : defaultCharset.Name);
							return false;
						}
						decoder = null;
					}
					else
					{
						decoder = defaultCharset.GetEncoding().GetDecoder();
					}
				}
				if (decoder != null)
				{
					this.ConvertRawFragment(currentPosition, this.iterator.CurrentPosition, decoder, allowControlCharacters, ref array, stringBuilder);
				}
				else
				{
					this.ZeroExpandFragment(currentPosition, this.iterator.CurrentPosition, allowControlCharacters, stringBuilder);
				}
			}
			ScratchPad.ReleaseStringBuilder();
			value = stringBuilder.ToString();
			return true;
		}

		private static bool Is2047Token(byte bT)
		{
			return bT < 128 && 0 != (byte)(ByteEncoder.Tables.CharacterTraits[(int)bT] & ByteEncoder.Tables.CharClasses.Token2047);
		}

		private static void RemoveProhibitedControlCharacters(char[] charBuffer, int offset, int length)
		{
			while (length != 0)
			{
				char c = charBuffer[offset];
				if (c < ' ')
				{
					charBuffer[offset] = ValueDecoder.ReplaceProhibitedControlCharacter(c);
				}
				offset++;
				length--;
			}
		}

		private static char ReplaceProhibitedControlCharacter(char ch)
		{
			foreach (char c in ValueDecoder.ControlCharacters)
			{
				if (c == ch)
				{
					return ' ';
				}
			}
			return ch;
		}

		private bool ParseEncodedWord(string lastEncodedWordCharsetName, string lastEncodedWordLanguage, ref byte[] byteBuffer, out string encodedWordCharsetName, out string encodedWordLanguage, out byte bOrQ, out ValuePosition encodedWordContentStart, out ValuePosition encodedWordContentEnd)
		{
			encodedWordCharsetName = null;
			encodedWordLanguage = null;
			bOrQ = 0;
			ValuePosition valuePosition = default(ValuePosition);
			encodedWordContentEnd = valuePosition;
			encodedWordContentStart = (encodedWordContentEnd = valuePosition);
			int num = this.iterator.Get();
			if (this.iterator.Get() != 63)
			{
				return false;
			}
			if (byteBuffer == null)
			{
				byteBuffer = ScratchPad.GetByteBuffer(Math.Max(this.maxCharsetNameLength + 1, Math.Min(1024, this.iterator.TotalLength)));
			}
			int num2 = -1;
			int i;
			for (i = 0; i < this.maxCharsetNameLength + 1; i++)
			{
				num = this.iterator.Get();
				if (!ValueDecoder.Is2047Token((byte)num))
				{
					break;
				}
				byteBuffer[i] = (byte)num;
				if (num2 == -1 && num == 42)
				{
					num2 = i;
				}
			}
			if (i == this.maxCharsetNameLength + 1 || num != 63 || i == 0 || num2 == 0)
			{
				return false;
			}
			num = this.iterator.Get();
			bOrQ = ((num == 66 || num == 98) ? 66 : ((num == 81 || num == 113) ? 81 : 0));
			if (bOrQ == 0 || this.iterator.Get() != 63)
			{
				return false;
			}
			if (num2 != -1)
			{
				int num3 = num2 + 1;
				int num4 = i - (num2 + 1);
				i = num2;
				if (num4 != 0)
				{
					if (lastEncodedWordLanguage != null && num4 == lastEncodedWordLanguage.Length)
					{
						int num5 = 0;
						while (num5 < num4 && lastEncodedWordLanguage[num5] == (char)byteBuffer[num3 + num5])
						{
							num5++;
						}
						if (num5 != num4)
						{
							encodedWordLanguage = ByteString.BytesToString(byteBuffer, num3, num4, false);
						}
						else
						{
							encodedWordLanguage = lastEncodedWordLanguage;
						}
					}
					else
					{
						encodedWordLanguage = ByteString.BytesToString(byteBuffer, num3, num4, false);
					}
				}
			}
			if (lastEncodedWordCharsetName != null && i == lastEncodedWordCharsetName.Length)
			{
				int num6 = 0;
				while (num6 < i && lastEncodedWordCharsetName[num6] == (char)byteBuffer[num6])
				{
					num6++;
				}
				if (num6 != i)
				{
					encodedWordCharsetName = ByteString.BytesToString(byteBuffer, 0, i, false);
				}
				else
				{
					encodedWordCharsetName = lastEncodedWordCharsetName;
				}
			}
			else
			{
				encodedWordCharsetName = ByteString.BytesToString(byteBuffer, 0, i, false);
			}
			encodedWordContentStart = this.iterator.CurrentPosition;
			bool flag = false;
			for (;;)
			{
				encodedWordContentEnd = this.iterator.CurrentPosition;
				num = this.iterator.Get();
				if (num == -1)
				{
					break;
				}
				if (MimeScan.IsLWSP((byte)num))
				{
					flag = true;
				}
				else
				{
					if (num == 63)
					{
						num = this.iterator.Get();
						if (num == -1)
						{
							return false;
						}
						if (num == 61)
						{
							return true;
						}
						this.iterator.Unget();
						if (bOrQ != 81)
						{
							return false;
						}
					}
					else if (num == 61 && flag)
					{
						num = this.iterator.Get();
						if (num == -1)
						{
							return false;
						}
						if (num == 63)
						{
							goto Block_33;
						}
						this.iterator.Unget();
					}
					flag = false;
				}
			}
			return false;
			Block_33:
			this.iterator.Unget();
			this.iterator.Unget();
			return false;
		}

		private void DecodeEncodedWord(byte bOrQ, Decoder decoder, ValuePosition encodedWordContentStart, ValuePosition encodedWordContentEnd, bool allowControlCharacters, ref byte[] byteBuffer, ref char[] charBuffer, StringBuilder sb)
		{
			ValueIterator valueIterator = new ValueIterator(this.iterator.Lines, this.iterator.LinesMask, encodedWordContentStart, encodedWordContentEnd);
			if (charBuffer == null)
			{
				charBuffer = ScratchPad.GetCharBuffer(Math.Min(1024, this.iterator.TotalLength));
			}
			if (byteBuffer == null)
			{
				byteBuffer = ScratchPad.GetByteBuffer(Math.Max(this.maxCharsetNameLength + 1, Math.Min(1024, this.iterator.TotalLength)));
			}
			int num = 0;
			if (bOrQ == 66)
			{
				int num2 = 0;
				int num3 = 0;
				while (!valueIterator.Eof)
				{
					byte b = (byte)(valueIterator.Get() - 32);
					if ((int)b < ByteEncoder.Tables.Base64ToByte.Length)
					{
						b = ByteEncoder.Tables.Base64ToByte[(int)b];
						if (b < 64)
						{
							num3 = (num3 << 6 | (int)b);
							num2++;
							if (num2 == 4)
							{
								byteBuffer[num++] = (byte)(num3 >> 16);
								byteBuffer[num++] = (byte)(num3 >> 8);
								byteBuffer[num++] = (byte)num3;
								num2 = 0;
								if (num + 3 >= byteBuffer.Length)
								{
									this.FlushDecodedBytes(byteBuffer, num, decoder, allowControlCharacters, charBuffer, sb);
									num = 0;
								}
							}
						}
					}
				}
				if (num2 != 0)
				{
					if (num2 == 2)
					{
						num3 <<= 12;
						byteBuffer[num++] = (byte)(num3 >> 16);
					}
					else if (num2 == 3)
					{
						num3 <<= 6;
						byteBuffer[num++] = (byte)(num3 >> 16);
						byteBuffer[num++] = (byte)(num3 >> 8);
					}
				}
			}
			else
			{
				while (!valueIterator.Eof)
				{
					byte b2 = (byte)valueIterator.Get();
					if (b2 == 61)
					{
						int num4 = valueIterator.Get();
						int num5 = valueIterator.Get();
						num4 = (int)((num4 < 0) ? byte.MaxValue : ByteEncoder.Tables.NumFromHex[num4]);
						num5 = (int)((num5 < 0) ? byte.MaxValue : ByteEncoder.Tables.NumFromHex[num5]);
						if (num4 == 255 || num5 == 255)
						{
							b2 = 61;
						}
						else
						{
							b2 = (byte)(num4 << 4 | num5);
						}
					}
					else if (b2 == 95)
					{
						b2 = 32;
					}
					byteBuffer[num++] = b2;
					if (num >= byteBuffer.Length)
					{
						this.FlushDecodedBytes(byteBuffer, num, decoder, allowControlCharacters, charBuffer, sb);
						num = 0;
					}
				}
			}
			if (num != 0)
			{
				this.FlushDecodedBytes(byteBuffer, num, decoder, allowControlCharacters, charBuffer, sb);
			}
		}

		private void FlushDecodedBytes(byte[] byteBuffer, int byteBufferLength, Decoder decoder, bool allowControlCharacters, char[] charBuffer, StringBuilder sb)
		{
			int num = 0;
			bool flag;
			do
			{
				int num2;
				int num3;
				decoder.Convert(byteBuffer, num, byteBufferLength, charBuffer, 0, charBuffer.Length, false, out num2, out num3, out flag);
				if (num3 != 0)
				{
					if (!allowControlCharacters)
					{
						ValueDecoder.RemoveProhibitedControlCharacters(charBuffer, 0, num3);
					}
					sb.Append(charBuffer, 0, num3);
				}
				num += num2;
				byteBufferLength -= num2;
			}
			while (!flag);
		}

		private void FlushDecoder(Decoder decoder, bool allowControlCharacters, ref byte[] byteBuffer, ref char[] charBuffer, StringBuilder sb)
		{
			int num;
			int num2;
			bool flag;
			decoder.Convert(byteBuffer, 0, 0, charBuffer, 0, charBuffer.Length, true, out num, out num2, out flag);
			if (num2 != 0)
			{
				if (!allowControlCharacters)
				{
					ValueDecoder.RemoveProhibitedControlCharacters(charBuffer, 0, num2);
				}
				sb.Append(charBuffer, 0, num2);
			}
		}

		private void ParseRawFragment(ref bool whitespaceOnly)
		{
			while (!this.iterator.Eof && 61 != this.iterator.Pick())
			{
				int num = MimeScan.SkipLwsp(this.iterator.Bytes, this.iterator.Offset, this.iterator.Length);
				if (num != 0)
				{
					this.iterator.Get(num);
				}
				if (this.iterator.Eof)
				{
					break;
				}
				if (61 == this.iterator.Pick())
				{
					return;
				}
				num = MimeScan.SkipToLwspOrEquals(this.iterator.Bytes, this.iterator.Offset, this.iterator.Length);
				if (num != 0)
				{
					whitespaceOnly = false;
					this.iterator.Get(num);
				}
			}
		}

		private Charset DetectValueCharset(Charset defaultCharset, bool enableJisDetection, bool enableUtf8Detection, bool enableDbcsDetection, out EncodingScheme encodingScheme)
		{
			ValueIterator valueIterator = new ValueIterator(this.iterator.Lines, this.iterator.LinesMask);
			FeInboundCharsetDetector feInboundCharsetDetector = new FeInboundCharsetDetector(defaultCharset.CodePage, false, enableJisDetection, enableUtf8Detection, enableDbcsDetection);
			while (!valueIterator.Eof)
			{
				feInboundCharsetDetector.AddBytes(valueIterator.Bytes, valueIterator.Offset, valueIterator.Length, false);
				valueIterator.Get(valueIterator.Length);
			}
			feInboundCharsetDetector.AddBytes(null, 0, 0, true);
			int codePageChoice = feInboundCharsetDetector.GetCodePageChoice();
			if (codePageChoice != defaultCharset.CodePage)
			{
				defaultCharset = Charset.GetCharset(codePageChoice);
			}
			if (!feInboundCharsetDetector.PureAscii)
			{
				if (feInboundCharsetDetector.Iso2022JpLikely || feInboundCharsetDetector.Iso2022KrLikely)
				{
					encodingScheme = EncodingScheme.Jis;
				}
				else
				{
					encodingScheme = EncodingScheme.EightBit;
				}
			}
			else if (defaultCharset.Name == "iso-2022-jp" && !feInboundCharsetDetector.Iso2022KrLikely)
			{
				encodingScheme = EncodingScheme.Jis;
			}
			else
			{
				encodingScheme = EncodingScheme.None;
			}
			return defaultCharset;
		}

		private void ZeroExpandFragment(ValuePosition start, ValuePosition end, bool allowControlCharacters, StringBuilder sb)
		{
			ValueIterator valueIterator = new ValueIterator(this.iterator.Lines, this.iterator.LinesMask, start, end);
			while (!valueIterator.Eof)
			{
				byte b = (byte)valueIterator.Get();
				if (!allowControlCharacters && b < 32)
				{
					b = (byte)ValueDecoder.ReplaceProhibitedControlCharacter((char)b);
				}
				sb.Append((char)b);
			}
		}

		private void ConvertRawFragment(ValuePosition start, ValuePosition end, Decoder decoder, bool allowControlCharacters, ref char[] charBuffer, StringBuilder sb)
		{
			ValueIterator valueIterator = new ValueIterator(this.iterator.Lines, this.iterator.LinesMask, start, end);
			if (!valueIterator.Eof)
			{
				if (charBuffer == null)
				{
					charBuffer = ScratchPad.GetCharBuffer(Math.Min(1024, this.iterator.TotalLength));
				}
				int length;
				int num;
				bool flag;
				do
				{
					decoder.Convert(valueIterator.Bytes, valueIterator.Offset, valueIterator.Length, charBuffer, 0, charBuffer.Length, false, out length, out num, out flag);
					if (num != 0)
					{
						if (!allowControlCharacters)
						{
							ValueDecoder.RemoveProhibitedControlCharacters(charBuffer, 0, num);
						}
						sb.Append(charBuffer, 0, num);
					}
					valueIterator.Get(length);
				}
				while (!flag || !valueIterator.Eof);
				decoder.Convert(MimeString.EmptyByteArray, 0, 0, charBuffer, 0, charBuffer.Length, true, out length, out num, out flag);
				if (num != 0)
				{
					if (!allowControlCharacters)
					{
						ValueDecoder.RemoveProhibitedControlCharacters(charBuffer, 0, num);
					}
					sb.Append(charBuffer, 0, num);
				}
			}
		}

		private const int MaxCharsetNameLength = 60;

		private const int MaxLanguageNameLength = 10;

		private static readonly char[] ControlCharacters = new char[]
		{
			'\0',
			'\r',
			'\n',
			'\f',
			'\v'
		};

		private int maxCharsetNameLength;

		private ValueIterator iterator;
	}
}
