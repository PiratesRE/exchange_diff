using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal static class RtfSupport
	{
		public static ushort CodePageFromCharRep(RtfSupport.CharRep charRep)
		{
			if ((int)charRep >= RtfSupport.CodePage.Length)
			{
				return 0;
			}
			return RtfSupport.CodePage[(int)charRep];
		}

		public static RtfSupport.CharRep CharRepFromLanguage(int langid)
		{
			short num = (short)(langid & 1023);
			if (num >= 26)
			{
				if (langid == 3098 || langid == 2092 || langid == 2115 || langid == 1104)
				{
					return RtfSupport.CharRep.RUSSIAN_INDEX;
				}
				if ((int)num >= RtfSupport.CharRepFromLID.Length)
				{
					return RtfSupport.CharRep.ANSI_INDEX;
				}
			}
			RtfSupport.CharRep charRep = RtfSupport.CharRepFromLID[(int)num];
			if (!RtfSupport.IsFECharRep(charRep))
			{
				return charRep;
			}
			if (charRep == RtfSupport.CharRep.GB2312_INDEX && langid != 2052 && langid != 4100)
			{
				charRep = RtfSupport.CharRep.BIG5_INDEX;
			}
			return charRep;
		}

		public static RtfSupport.CharRep CharRepFromCharSet(int charset)
		{
			byte b = 0;
			while ((int)b < RtfSupport.CharSet.Length)
			{
				if ((int)RtfSupport.CharSet[(int)b] == charset)
				{
					return (RtfSupport.CharRep)b;
				}
				b += 1;
			}
			return RtfSupport.CharRep.UNDEFINED;
		}

		public static RtfSupport.CharRep CharRepFromCodePage(ushort codePage)
		{
			byte b = 0;
			while ((int)b < RtfSupport.CodePage.Length)
			{
				if (RtfSupport.CodePage[(int)b] == codePage)
				{
					return (RtfSupport.CharRep)b;
				}
				b += 1;
			}
			if (codePage == 54936)
			{
				return RtfSupport.CharRep.GB18030_INDEX;
			}
			return RtfSupport.CharRep.UNDEFINED;
		}

		public static int CharSetFromCodePage(ushort codePage)
		{
			byte b = 0;
			while ((int)b < RtfSupport.CodePage.Length)
			{
				if (RtfSupport.CodePage[(int)b] == codePage)
				{
					return (int)RtfSupport.CharSet[(int)b];
				}
				b += 1;
			}
			return 1;
		}

		public static bool IsBiDiCharRep(RtfSupport.CharRep charRep)
		{
			return (RtfSupport.CharRep.HEBREW_INDEX <= charRep && charRep <= RtfSupport.CharRep.ARABIC_INDEX) || (RtfSupport.CharRep.SYRIAC_INDEX <= charRep && charRep <= RtfSupport.CharRep.THAANA_INDEX);
		}

		public static bool IsRtlCharSet(int charset)
		{
			return 177 <= charset && charset <= 178;
		}

		public static bool IsBiDiLanguage(int langid)
		{
			short num = (short)(langid & 1023);
			return num == 1 || num == 13 || num == 32 || num == 41 || num == 90 || num == 101;
		}

		public static bool IsHebrewLanguage(int langid)
		{
			short num = (short)(langid & 1023);
			return num == 13;
		}

		public static bool IsArabicLanguage(int langid)
		{
			short num = (short)(langid & 1023);
			return num == 1 || num == 32 || num == 41 || num == 90 || num == 101;
		}

		public static bool IsThaiLanguage(int langid)
		{
			short num = (short)(langid & 1023);
			return num == 30;
		}

		public static bool IsFECharRep(RtfSupport.CharRep charRep)
		{
			return RtfSupport.CharRep.SHIFTJIS_INDEX <= charRep && charRep <= RtfSupport.CharRep.BIG5_INDEX;
		}

		public static int RGB(int red, int green, int blue)
		{
			return (int)((byte)blue) << 16 | (int)((byte)green) << 8 | (int)((byte)red);
		}

		public static int Unescape(byte b1, byte b2)
		{
			if (ParseSupport.HexCharacter(ParseSupport.GetCharClass((char)b1)) && ParseSupport.HexCharacter(ParseSupport.GetCharClass((char)b2)))
			{
				return ParseSupport.CharToHex((char)b1) << 4 | ParseSupport.CharToHex((char)b2);
			}
			return 256;
		}

		public static void Escape(char ch, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)"0123456789ABCDEF"[(int)(ch >> 4 & '\u000f')];
			buffer[offset + 1] = (byte)"0123456789ABCDEF"[(int)(ch & '\u000f')];
		}

		public static bool IsHyperlinkField(ref ScratchBuffer scratch, out bool local, out BufferString linkUrl)
		{
			int num = 0;
			int length = scratch.Length;
			while (num != scratch.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[num])))
			{
				num++;
			}
			if (scratch.Length - num > 10 && scratch[num] == 'H' && scratch[num + 1] == 'Y' && scratch[num + 2] == 'P' && scratch[num + 3] == 'E' && scratch[num + 4] == 'R' && scratch[num + 5] == 'L' && scratch[num + 6] == 'I' && scratch[num + 7] == 'N' && scratch[num + 8] == 'K' && scratch[num + 9] == ' ')
			{
				num += 10;
				int num2;
				int num3;
				int num4;
				int num5;
				int fieldArgument = RtfSupport.GetFieldArgument(ref scratch, num, out num2, out num3, out num4, out num5);
				if (num3 == 2 && scratch[num2] == '\\' && scratch[num2 + 1] == 'l')
				{
					local = true;
					num += fieldArgument;
					fieldArgument = RtfSupport.GetFieldArgument(ref scratch, num, out num2, out num3, out num4, out num5);
				}
				else
				{
					local = false;
				}
				if (num5 != 0)
				{
					if (local)
					{
						num4--;
						num5++;
						scratch[num4] = '#';
					}
					linkUrl = scratch.SubString(num4, num5);
					return true;
				}
			}
			local = false;
			linkUrl = BufferString.Null;
			return false;
		}

		public static bool IsIncludePictureField(ref ScratchBuffer scratch, out BufferString linkUrl)
		{
			int num = 0;
			int length = scratch.Length;
			while (num != scratch.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[num])))
			{
				num++;
			}
			if (scratch.Length - num > 15 && scratch[num] == 'I' && scratch[num + 1] == 'N' && scratch[num + 2] == 'C' && scratch[num + 3] == 'L' && scratch[num + 4] == 'U' && scratch[num + 5] == 'D' && scratch[num + 6] == 'E' && scratch[num + 7] == 'P' && scratch[num + 8] == 'I' && scratch[num + 9] == 'C' && scratch[num + 10] == 'T' && scratch[num + 11] == 'U' && scratch[num + 12] == 'R' && scratch[num + 13] == 'E' && scratch[num + 14] == ' ')
			{
				num += 15;
				int offset;
				int num2;
				int offset2;
				int num3;
				int fieldArgument = RtfSupport.GetFieldArgument(ref scratch, num, out offset, out num2, out offset2, out num3);
				while (num2 == 2 && scratch[offset] == '\\')
				{
					num += fieldArgument;
					fieldArgument = RtfSupport.GetFieldArgument(ref scratch, num, out offset, out num2, out offset2, out num3);
				}
				if (num3 > 2)
				{
					linkUrl = scratch.SubString(offset2, num3);
					return true;
				}
			}
			linkUrl = BufferString.Null;
			return false;
		}

		public static bool IsSymbolField(ref ScratchBuffer scratch, out TextMapping textMapping, out char symbol, out short points)
		{
			textMapping = TextMapping.Unicode;
			symbol = '\0';
			points = 0;
			int num = 0;
			int length = scratch.Length;
			while (num != scratch.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[num])))
			{
				num++;
			}
			if (scratch.Length - num <= 7 || scratch[num] != 'S' || scratch[num + 1] != 'Y' || scratch[num + 2] != 'M' || scratch[num + 3] != 'B' || scratch[num + 4] != 'O' || scratch[num + 5] != 'L' || scratch[num + 6] != ' ')
			{
				return false;
			}
			num += 7;
			int num2;
			int num3;
			int num4;
			int num5;
			int fieldArgument = RtfSupport.GetFieldArgument(ref scratch, num, out num2, out num3, out num4, out num5);
			int num6;
			char c;
			if (num3 > 2 && scratch.Buffer[num2] == '0' && (ushort)(scratch.Buffer[num2 + 1] | ' ') == 120)
			{
				num6 = 2;
				while (num6 < num3 && (c = scratch.Buffer[num2 + num6]) <= 'f')
				{
					if (('0' > c || c > '9') && ('a' > c || c > 'f'))
					{
						if ('A' > c)
						{
							break;
						}
						if (c > 'F')
						{
							break;
						}
					}
					symbol = (symbol << 4) + ((c <= '9') ? (c - '0') : ((c & 'O') - 'A' + '\n'));
					num6++;
				}
			}
			else
			{
				num6 = 0;
				while (num6 < num3 && (c = scratch.Buffer[num2 + num6]) <= '9' && '0' <= c)
				{
					symbol = '\n' * symbol + (c - '0');
					num6++;
				}
			}
			num += fieldArgument;
			fieldArgument = RtfSupport.GetFieldArgument(ref scratch, num, out num2, out num3, out num4, out num5);
			if (num3 != 2 || scratch[num2] != '\\' || scratch[num2 + 1] != 'f')
			{
				return false;
			}
			num += fieldArgument;
			fieldArgument = RtfSupport.GetFieldArgument(ref scratch, num, out num2, out num3, out num4, out num5);
			if (num5 == 0)
			{
				return false;
			}
			RecognizeInterestingFontName recognizeInterestingFontName = default(RecognizeInterestingFontName);
			num6 = 0;
			while (num6 < num5 && !recognizeInterestingFontName.IsRejected)
			{
				recognizeInterestingFontName.AddCharacter(scratch.Buffer[num4 + num6]);
				num6++;
			}
			textMapping = recognizeInterestingFontName.TextMapping;
			if (textMapping == TextMapping.Unicode)
			{
				textMapping = TextMapping.OtherSymbol;
			}
			num += fieldArgument;
			fieldArgument = RtfSupport.GetFieldArgument(ref scratch, num, out num2, out num3, out num4, out num5);
			if (num3 != 2 || scratch[num2] != '\\' || scratch[num2 + 1] != 's')
			{
				return true;
			}
			num += fieldArgument;
			fieldArgument = RtfSupport.GetFieldArgument(ref scratch, num, out num2, out num3, out num4, out num5);
			num6 = 0;
			while (num6 < num3 && (c = scratch.Buffer[num2 + num6]) <= '9' && '0' <= c)
			{
				points = 10 * points + (short)(c - '0');
				num6++;
			}
			return true;
		}

		private static int GetFieldArgument(ref ScratchBuffer scratch, int offset, out int rawResultOffset, out int rawResultLength, out int unescapedResultOffset, out int unescapedResultLength)
		{
			int length = scratch.Length;
			bool flag = false;
			int num = 0;
			while (offset < length && scratch[offset] == ' ')
			{
				offset++;
				num++;
			}
			if (offset < length && scratch[offset] == '"')
			{
				flag = true;
				offset++;
				num++;
			}
			rawResultOffset = offset;
			rawResultLength = 0;
			unescapedResultOffset = length;
			unescapedResultLength = 0;
			while (offset < length)
			{
				char c = scratch[offset];
				if ((c == '"' && flag) || (c == ' ' && !flag))
				{
					num++;
					break;
				}
				if (c == '\\')
				{
					offset++;
					num++;
					rawResultLength++;
					if (offset == length)
					{
						break;
					}
					c = scratch[offset];
				}
				if (scratch.Append(c, 5120) != 0)
				{
					unescapedResultLength++;
				}
				rawResultLength++;
				offset++;
				num++;
			}
			scratch.Length = length;
			return num;
		}

		public static string StringFontNameFromScratch(ScratchBuffer scratch)
		{
			int num = 0;
			int num2;
			for (num2 = scratch.Length; num2 != 0; num2--)
			{
				if (scratch[num2 - 1] != ';' && !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[num2 - 1])))
				{
					break;
				}
			}
			while (num2 != 0 && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(scratch[num])))
			{
				num++;
				num2--;
			}
			if (num2 != 0 && scratch[num] != '?')
			{
				return scratch.ToString(num, num2);
			}
			return null;
		}

		public const int RtfNestingLimit = 4096;

		public const int MaxBookmarkNameLength = 4096;

		public const int MaxFieldInstructionLength = 4096;

		public const int MaxFontNameLength = 256;

		public const int MaxUrlLength = 1024;

		public const int MaxShapePropertyName = 128;

		public const int MaxShapePropertyValue = 4096;

		public const byte LANG_NEUTRAL = 0;

		public const byte LANG_AFRIKAANS = 54;

		public const byte LANG_ALBANIAN = 28;

		public const byte LANG_ARABIC = 1;

		public const byte LANG_BASQUE = 45;

		public const byte LANG_BELARUSIAN = 35;

		public const byte LANG_BULGARIAN = 2;

		public const byte LANG_CATALAN = 3;

		public const byte LANG_CHINESE = 4;

		public const byte LANG_CROATIAN = 26;

		public const byte LANG_CZECH = 5;

		public const byte LANG_DANISH = 6;

		public const byte LANG_DUTCH = 19;

		public const byte LANG_ENGLISH = 9;

		public const byte LANG_ESTONIAN = 37;

		public const byte LANG_FAEROESE = 56;

		public const byte LANG_FARSI = 41;

		public const byte LANG_FINNISH = 11;

		public const byte LANG_FRENCH = 12;

		public const byte LANG_GERMAN = 7;

		public const byte LANG_GREEK = 8;

		public const byte LANG_HEBREW = 13;

		public const byte LANG_HUNGARIAN = 14;

		public const byte LANG_ICELANDIC = 15;

		public const byte LANG_INDONESIAN = 33;

		public const byte LANG_ITALIAN = 16;

		public const byte LANG_JAPANESE = 17;

		public const byte LANG_KOREAN = 18;

		public const byte LANG_LATVIAN = 38;

		public const byte LANG_LITHUANIAN = 39;

		public const byte LANG_NORWEGIAN = 20;

		public const byte LANG_POLISH = 21;

		public const byte LANG_PORTUGUESE = 22;

		public const byte LANG_ROMANIAN = 24;

		public const byte LANG_RUSSIAN = 25;

		public const byte LANG_SERBIAN = 26;

		public const byte LANG_SLOVAK = 27;

		public const byte LANG_SLOVENIAN = 36;

		public const byte LANG_SPANISH = 10;

		public const byte LANG_SWEDISH = 29;

		public const byte LANG_THAI = 30;

		public const byte LANG_TURKISH = 31;

		public const byte LANG_UKRAINIAN = 34;

		public const byte LANG_VIETNAMESE = 42;

		public const byte LANG_URDU = 32;

		public const byte LANG_SYRIAC = 90;

		public const byte LANG_DIVEHI = 101;

		public const short LID_SERBIAN_CYRILLIC = 3098;

		public const short LID_AZERI_CYRILLIC = 2092;

		public const short LID_UZBEK_CYRILLIC = 2115;

		public const short LID_MONGOLIAN_CYRILLIC = 1104;

		public const short LID_PRC = 2052;

		public const short LID_SINGAPORE = 4100;

		public const byte ANSI_CHARSET = 0;

		public const byte DEFAULT_CHARSET = 1;

		public const byte SYMBOL_CHARSET = 2;

		public const byte SHIFTJIS_CHARSET = 128;

		public const byte HANGEUL_CHARSET = 129;

		public const byte HANGUL_CHARSET = 129;

		public const byte GB2312_CHARSET = 134;

		public const byte CHINESEBIG5_CHARSET = 136;

		public const byte OEM_CHARSET = 255;

		public const byte JOHAB_CHARSET = 130;

		public const byte HEBREW_CHARSET = 177;

		public const byte ARABIC_CHARSET = 178;

		public const byte ARABIC1_CHARSET = 180;

		public const byte GREEK_CHARSET = 161;

		public const byte TURKISH_CHARSET = 162;

		public const byte VIETNAMESE_CHARSET = 163;

		public const byte THAI_CHARSET = 222;

		public const byte EASTEUROPE_CHARSET = 238;

		public const byte RUSSIAN_CHARSET = 204;

		public const byte MAC_CHARSET = 77;

		public const byte BALTIC_CHARSET = 186;

		public const byte PC437_CHARSET = 254;

		public const ushort CP_SYMBOL = 42;

		private const string HexCharacters = "0123456789ABCDEF";

		private static readonly RtfSupport.CharRep[] CharRepFromLID = new RtfSupport.CharRep[]
		{
			RtfSupport.CharRep.DEFAULT_INDEX,
			RtfSupport.CharRep.ARABIC_INDEX,
			RtfSupport.CharRep.RUSSIAN_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.GB2312_INDEX,
			RtfSupport.CharRep.EASTEUROPE_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.GREEK_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.HEBREW_INDEX,
			RtfSupport.CharRep.EASTEUROPE_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.SHIFTJIS_INDEX,
			RtfSupport.CharRep.HANGUL_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.EASTEUROPE_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.DEFAULT_INDEX,
			RtfSupport.CharRep.EASTEUROPE_INDEX,
			RtfSupport.CharRep.RUSSIAN_INDEX,
			RtfSupport.CharRep.EASTEUROPE_INDEX,
			RtfSupport.CharRep.EASTEUROPE_INDEX,
			RtfSupport.CharRep.EASTEUROPE_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.THAI_INDEX,
			RtfSupport.CharRep.TURKISH_INDEX,
			RtfSupport.CharRep.ARABIC_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.RUSSIAN_INDEX,
			RtfSupport.CharRep.RUSSIAN_INDEX,
			RtfSupport.CharRep.EASTEUROPE_INDEX,
			RtfSupport.CharRep.BALTIC_INDEX,
			RtfSupport.CharRep.BALTIC_INDEX,
			RtfSupport.CharRep.BALTIC_INDEX,
			RtfSupport.CharRep.DEFAULT_INDEX,
			RtfSupport.CharRep.ARABIC_INDEX,
			RtfSupport.CharRep.VIET_INDEX,
			RtfSupport.CharRep.NCHARSETS,
			RtfSupport.CharRep.TURKISH_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.DEFAULT_INDEX,
			RtfSupport.CharRep.RUSSIAN_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.GEORGIAN_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.DEVANAGARI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.HEBREW_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.RUSSIAN_INDEX,
			RtfSupport.CharRep.RUSSIAN_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.TURKISH_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.BENGALI_INDEX,
			RtfSupport.CharRep.GURMUKHI_INDEX,
			RtfSupport.CharRep.GUJARATI_INDEX,
			RtfSupport.CharRep.ORIYA_INDEX,
			RtfSupport.CharRep.TAMIL_INDEX,
			RtfSupport.CharRep.TELUGU_INDEX,
			RtfSupport.CharRep.KANNADA_INDEX,
			RtfSupport.CharRep.MALAYALAM_INDEX,
			RtfSupport.CharRep.BENGALI_INDEX,
			RtfSupport.CharRep.DEVANAGARI_INDEX,
			RtfSupport.CharRep.DEVANAGARI_INDEX,
			RtfSupport.CharRep.MONGOLIAN_INDEX,
			RtfSupport.CharRep.TIBETAN_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.KHMER_INDEX,
			RtfSupport.CharRep.LAO_INDEX,
			RtfSupport.CharRep.MYANMAR_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.DEVANAGARI_INDEX,
			RtfSupport.CharRep.BENGALI_INDEX,
			RtfSupport.CharRep.GURMUKHI_INDEX,
			RtfSupport.CharRep.SYRIAC_INDEX,
			RtfSupport.CharRep.SINHALA_INDEX,
			RtfSupport.CharRep.CHEROKEE_INDEX,
			RtfSupport.CharRep.ABORIGINAL_INDEX,
			RtfSupport.CharRep.ETHIOPIC_INDEX,
			RtfSupport.CharRep.DEFAULT_INDEX,
			RtfSupport.CharRep.DEFAULT_INDEX,
			RtfSupport.CharRep.DEVANAGARI_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.ARABIC_INDEX,
			RtfSupport.CharRep.ANSI_INDEX,
			RtfSupport.CharRep.THAANA_INDEX
		};

		private static readonly ushort[] CodePage = new ushort[]
		{
			1252,
			1250,
			1251,
			1253,
			1254,
			1255,
			1256,
			1257,
			1258,
			0,
			42,
			874,
			932,
			936,
			949,
			950,
			437,
			850,
			10000,
			1256
		};

		private static readonly byte[] CharSet = new byte[]
		{
			0,
			238,
			204,
			161,
			162,
			177,
			178,
			186,
			163,
			1,
			2,
			222,
			128,
			134,
			129,
			136,
			254,
			byte.MaxValue,
			77,
			180
		};

		public static readonly byte[] UnsafeAsciiMap = new byte[]
		{
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1
		};

		public enum CharRep : byte
		{
			ANSI_INDEX,
			EASTEUROPE_INDEX,
			RUSSIAN_INDEX,
			GREEK_INDEX,
			TURKISH_INDEX,
			HEBREW_INDEX,
			ARABIC_INDEX,
			BALTIC_INDEX,
			VIET_INDEX,
			DEFAULT_INDEX,
			SYMBOL_INDEX,
			THAI_INDEX,
			SHIFTJIS_INDEX,
			GB2312_INDEX,
			HANGUL_INDEX,
			BIG5_INDEX,
			PC437_INDEX,
			OEM_INDEX,
			MAC_INDEX,
			ARABIC1_INDEX,
			NCHARSETS,
			ARMENIAN_INDEX = 20,
			SYRIAC_INDEX,
			THAANA_INDEX,
			DEVANAGARI_INDEX,
			BENGALI_INDEX,
			GURMUKHI_INDEX,
			GUJARATI_INDEX,
			ORIYA_INDEX,
			TAMIL_INDEX,
			TELUGU_INDEX,
			KANNADA_INDEX,
			MALAYALAM_INDEX,
			SINHALA_INDEX,
			LAO_INDEX,
			TIBETAN_INDEX,
			MYANMAR_INDEX,
			GEORGIAN_INDEX,
			JAMO_INDEX,
			ETHIOPIC_INDEX,
			CHEROKEE_INDEX,
			ABORIGINAL_INDEX,
			OGHAM_INDEX,
			RUNIC_INDEX,
			KHMER_INDEX,
			MONGOLIAN_INDEX,
			BRAILLE_INDEX,
			YI_INDEX,
			JPN2_INDEX,
			CHS2_INDEX,
			KOR2_INDEX,
			CHT2_INDEX,
			GB18030_INDEX,
			NCHARREPERTOIRES,
			UNDEFINED = 255
		}
	}
}
