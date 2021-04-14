using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnicodeToAnsiConverter
	{
		public static string Convert(string input, bool skipInvalidCharacter, char invalidCharacterReplacement)
		{
			Encoding encoding = Encoding.GetEncoding(1252, new UnicodeToAnsiConverter.EncoderAnsiBestFitFallback(skipInvalidCharacter, invalidCharacterReplacement), UnicodeToAnsiConverter.DefaultAnsiEncoding.DecoderFallback);
			return encoding.GetString(encoding.GetBytes(input));
		}

		private const int CP_ANSI = 1252;

		private static readonly Encoding DefaultAnsiEncoding = Encoding.GetEncoding(1252);

		private struct UnicodeToAnsiBestFitMapper
		{
			private UnicodeToAnsiBestFitMapper(int minCodePoint, int maxCodePoint, string[] mapping)
			{
				this.minCodePoint = minCodePoint;
				this.maxCodePoint = maxCodePoint;
				this.mapping = mapping;
			}

			public static string Map(int codePoint)
			{
				foreach (UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper unicodeToAnsiBestFitMapper in UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper.MappingTable)
				{
					if (unicodeToAnsiBestFitMapper.minCodePoint <= codePoint && unicodeToAnsiBestFitMapper.maxCodePoint >= codePoint)
					{
						return unicodeToAnsiBestFitMapper.mapping[codePoint - unicodeToAnsiBestFitMapper.minCodePoint];
					}
				}
				return null;
			}

			// Note: this type is marked as 'beforefieldinit'.
			static UnicodeToAnsiBestFitMapper()
			{
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[38];
				array[0] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(160, 383, new string[]
				{
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"a",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"2",
					"3",
					null,
					null,
					null,
					null,
					null,
					"1",
					"o",
					null,
					"1/4",
					"1/2",
					"3/4",
					null,
					"A",
					"A",
					"A",
					"A",
					"A",
					"A",
					null,
					"C",
					"E",
					"E",
					"E",
					"E",
					"I",
					"I",
					"I",
					"I",
					null,
					"N",
					"O",
					"O",
					"O",
					"O",
					"O",
					null,
					null,
					"U",
					"U",
					"U",
					"U",
					"Y",
					null,
					null,
					"a",
					"a",
					"a",
					"a",
					"a",
					"a",
					null,
					"c",
					"e",
					"e",
					"e",
					"e",
					"i",
					"i",
					"i",
					"i",
					null,
					"n",
					"o",
					"o",
					"o",
					"o",
					"o",
					null,
					null,
					"u",
					"u",
					"u",
					"u",
					"y",
					null,
					"y",
					"A",
					"a",
					"A",
					"a",
					"A",
					"a",
					"C",
					"c",
					"C",
					"c",
					"C",
					"c",
					"C",
					"c",
					"D",
					"d",
					null,
					null,
					"E",
					"e",
					"E",
					"e",
					"E",
					"e",
					"E",
					"e",
					"E",
					"e",
					"G",
					"g",
					"G",
					"g",
					"G",
					"g",
					"G",
					"g",
					"H",
					"h",
					null,
					null,
					"I",
					"i",
					"I",
					"i",
					"I",
					"i",
					"I",
					"i",
					"I",
					null,
					"IJ",
					"ij",
					"J",
					"j",
					"K",
					"k",
					null,
					"L",
					"l",
					"L",
					"l",
					"L",
					"l",
					"L·",
					"l·",
					null,
					null,
					"N",
					"n",
					"N",
					"n",
					"N",
					"n",
					"'n",
					null,
					null,
					"O",
					"o",
					"O",
					"o",
					"O",
					"o",
					null,
					null,
					"R",
					"r",
					"R",
					"r",
					"R",
					"r",
					"S",
					"s",
					"S",
					"s",
					"S",
					"s",
					"S",
					"s",
					"T",
					"t",
					"T",
					"t",
					null,
					null,
					"U",
					"u",
					"U",
					"u",
					"U",
					"u",
					"U",
					"u",
					"U",
					"u",
					"U",
					"u",
					"W",
					"w",
					"Y",
					"y",
					"Y",
					"Z",
					"z",
					"Z",
					"z",
					"Z",
					"z",
					"s"
				});
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array2 = array;
				int num = 1;
				int num2 = 416;
				int num3 = 575;
				string[] array3 = new string[160];
				array3[0] = "O";
				array3[1] = "o";
				array3[15] = "U";
				array3[16] = "u";
				array3[36] = "DŽ";
				array3[37] = "Dž";
				array3[38] = "dž";
				array3[39] = "LJ";
				array3[40] = "Lj";
				array3[41] = "lj";
				array3[42] = "NJ";
				array3[43] = "Nj";
				array3[44] = "nj";
				array3[45] = "A";
				array3[46] = "a";
				array3[47] = "I";
				array3[48] = "i";
				array3[49] = "O";
				array3[50] = "o";
				array3[51] = "U";
				array3[52] = "u";
				array3[53] = "Ü";
				array3[54] = "ü";
				array3[55] = "Ü";
				array3[56] = "ü";
				array3[57] = "Ü";
				array3[58] = "ü";
				array3[59] = "Ü";
				array3[60] = "ü";
				array3[62] = "Ä";
				array3[63] = "ä";
				array3[66] = "Æ";
				array3[67] = "æ";
				array3[70] = "G";
				array3[71] = "g";
				array3[72] = "K";
				array3[73] = "k";
				array3[74] = "O";
				array3[75] = "o";
				array3[76] = "O";
				array3[77] = "o";
				array3[80] = "j";
				array3[81] = "DZ";
				array3[82] = "Dz";
				array3[83] = "dz";
				array3[84] = "G";
				array3[85] = "g";
				array3[88] = "N";
				array3[89] = "n";
				array3[90] = "Å";
				array3[91] = "å";
				array3[92] = "Æ";
				array3[93] = "æ";
				array3[94] = "Ø";
				array3[95] = "ø";
				array3[96] = "A";
				array3[97] = "a";
				array3[98] = "A";
				array3[99] = "a";
				array3[100] = "E";
				array3[101] = "e";
				array3[102] = "E";
				array3[103] = "e";
				array3[104] = "I";
				array3[105] = "i";
				array3[106] = "I";
				array3[107] = "i";
				array3[108] = "O";
				array3[109] = "o";
				array3[110] = "O";
				array3[111] = "o";
				array3[112] = "R";
				array3[113] = "r";
				array3[114] = "R";
				array3[115] = "r";
				array3[116] = "U";
				array3[117] = "u";
				array3[118] = "U";
				array3[119] = "u";
				array3[120] = "S";
				array3[121] = "s";
				array3[122] = "T";
				array3[123] = "t";
				array3[126] = "H";
				array3[127] = "h";
				array3[134] = "A";
				array3[135] = "a";
				array3[136] = "E";
				array3[137] = "e";
				array3[138] = "Ö";
				array3[139] = "ö";
				array3[140] = "Õ";
				array3[141] = "õ";
				array3[142] = "O";
				array3[143] = "o";
				array3[146] = "Y";
				array3[147] = "y";
				array2[num] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num2, num3, array3);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array4 = array;
				int num4 = 2;
				int num5 = 688;
				int num6 = 703;
				string[] array5 = new string[16];
				array5[0] = "h";
				array5[2] = "j";
				array5[3] = "r";
				array5[7] = "w";
				array5[8] = "y";
				array4[num4] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num5, num6, array5);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array6 = array;
				int num7 = 3;
				int num8 = 736;
				int num9 = 751;
				string[] array7 = new string[16];
				array7[1] = "l";
				array7[2] = "s";
				array7[3] = "x";
				array6[num7] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num8, num9, array7);
				array[4] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(880, 911, new string[]
				{
					null,
					null,
					null,
					null,
					"'",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					";",
					null,
					null,
					null,
					null,
					null,
					null,
					"¨",
					null,
					"·",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"O"
				});
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array8 = array;
				int num10 = 5;
				int num11 = 928;
				int num12 = 943;
				string[] array9 = new string[16];
				array9[12] = "a";
				array9[13] = "e";
				array8[num10] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num11, num12, array9);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array10 = array;
				int num13 = 6;
				int num14 = 976;
				int num15 = 991;
				string[] array11 = new string[16];
				array11[0] = "ß";
				array11[5] = "f";
				array11[6] = "p";
				array10[num13] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num14, num15, array11);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array12 = array;
				int num16 = 7;
				int num17 = 1008;
				int num18 = 1023;
				string[] array13 = new string[16];
				array13[4] = "T";
				array13[5] = "e";
				array13[9] = "S";
				array12[num16] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num17, num18, array13);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array14 = array;
				int num19 = 8;
				int num20 = 7456;
				int num21 = 7535;
				string[] array15 = new string[80];
				array15[12] = "A";
				array15[13] = "Æ";
				array15[14] = "B";
				array15[16] = "D";
				array15[17] = "E";
				array15[19] = "G";
				array15[20] = "H";
				array15[21] = "I";
				array15[22] = "J";
				array15[23] = "K";
				array15[24] = "L";
				array15[25] = "M";
				array15[26] = "N";
				array15[28] = "O";
				array15[30] = "P";
				array15[31] = "R";
				array15[32] = "T";
				array15[33] = "U";
				array15[34] = "W";
				array15[35] = "a";
				array15[39] = "b";
				array15[40] = "d";
				array15[41] = "e";
				array15[45] = "g";
				array15[47] = "k";
				array15[48] = "m";
				array15[50] = "o";
				array15[54] = "p";
				array15[55] = "t";
				array15[56] = "u";
				array15[59] = "v";
				array15[61] = "ß";
				array15[63] = "d";
				array15[64] = "f";
				array15[66] = "i";
				array15[67] = "r";
				array15[68] = "u";
				array15[69] = "v";
				array15[70] = "ß";
				array15[73] = "f";
				array14[num19] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num20, num21, array15);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array16 = array;
				int num22 = 9;
				int num23 = 7568;
				int num24 = 7615;
				string[] array17 = new string[48];
				array17[12] = "c";
				array17[14] = "ð";
				array17[16] = "f";
				array17[18] = "g";
				array17[37] = "t";
				array17[43] = "z";
				array16[num22] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num23, num24, array17);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array18 = array;
				int num25 = 10;
				int num26 = 7680;
				int num27 = 7967;
				string[] array19 = new string[288];
				array19[0] = "A";
				array19[1] = "a";
				array19[2] = "B";
				array19[3] = "b";
				array19[4] = "B";
				array19[5] = "b";
				array19[6] = "B";
				array19[7] = "b";
				array19[8] = "Ç";
				array19[9] = "ç";
				array19[10] = "D";
				array19[11] = "d";
				array19[12] = "D";
				array19[13] = "d";
				array19[14] = "D";
				array19[15] = "d";
				array19[16] = "D";
				array19[17] = "d";
				array19[18] = "D";
				array19[19] = "d";
				array19[20] = "E";
				array19[21] = "e";
				array19[22] = "E";
				array19[23] = "e";
				array19[24] = "E";
				array19[25] = "e";
				array19[26] = "E";
				array19[27] = "e";
				array19[30] = "F";
				array19[31] = "f";
				array19[32] = "G";
				array19[33] = "g";
				array19[34] = "H";
				array19[35] = "h";
				array19[36] = "H";
				array19[37] = "h";
				array19[38] = "H";
				array19[39] = "h";
				array19[40] = "H";
				array19[41] = "h";
				array19[42] = "H";
				array19[43] = "h";
				array19[44] = "I";
				array19[45] = "i";
				array19[46] = "Ï";
				array19[47] = "ï";
				array19[48] = "K";
				array19[49] = "k";
				array19[50] = "K";
				array19[51] = "k";
				array19[52] = "K";
				array19[53] = "k";
				array19[54] = "L";
				array19[55] = "l";
				array19[58] = "L";
				array19[59] = "l";
				array19[60] = "L";
				array19[61] = "l";
				array19[62] = "M";
				array19[63] = "m";
				array19[64] = "M";
				array19[65] = "m";
				array19[66] = "M";
				array19[67] = "m";
				array19[68] = "N";
				array19[69] = "n";
				array19[70] = "N";
				array19[71] = "n";
				array19[72] = "N";
				array19[73] = "n";
				array19[74] = "N";
				array19[75] = "n";
				array19[76] = "Õ";
				array19[77] = "õ";
				array19[78] = "Õ";
				array19[79] = "õ";
				array19[80] = "O";
				array19[81] = "o";
				array19[82] = "O";
				array19[83] = "o";
				array19[84] = "P";
				array19[85] = "p";
				array19[86] = "P";
				array19[87] = "p";
				array19[88] = "R";
				array19[89] = "r";
				array19[90] = "R";
				array19[91] = "r";
				array19[94] = "R";
				array19[95] = "r";
				array19[96] = "S";
				array19[97] = "s";
				array19[98] = "S";
				array19[99] = "s";
				array19[100] = "S";
				array19[101] = "s";
				array19[102] = "Š";
				array19[103] = "š";
				array19[106] = "T";
				array19[107] = "t";
				array19[108] = "T";
				array19[109] = "t";
				array19[110] = "T";
				array19[111] = "t";
				array19[112] = "T";
				array19[113] = "t";
				array19[114] = "U";
				array19[115] = "u";
				array19[116] = "U";
				array19[117] = "u";
				array19[118] = "U";
				array19[119] = "u";
				array19[120] = "U";
				array19[121] = "u";
				array19[122] = "U";
				array19[123] = "u";
				array19[124] = "V";
				array19[125] = "v";
				array19[126] = "V";
				array19[127] = "v";
				array19[128] = "W";
				array19[129] = "w";
				array19[130] = "W";
				array19[131] = "w";
				array19[132] = "W";
				array19[133] = "w";
				array19[134] = "W";
				array19[135] = "w";
				array19[136] = "W";
				array19[137] = "w";
				array19[138] = "X";
				array19[139] = "x";
				array19[140] = "X";
				array19[141] = "x";
				array19[142] = "Y";
				array19[143] = "y";
				array19[144] = "Z";
				array19[145] = "z";
				array19[146] = "Z";
				array19[147] = "z";
				array19[148] = "Z";
				array19[149] = "z";
				array19[150] = "h";
				array19[151] = "t";
				array19[152] = "w";
				array19[153] = "y";
				array19[160] = "A";
				array19[161] = "a";
				array19[162] = "A";
				array19[163] = "a";
				array19[164] = "Â";
				array19[165] = "â";
				array19[166] = "Â";
				array19[167] = "â";
				array19[168] = "Â";
				array19[169] = "â";
				array19[170] = "Â";
				array19[171] = "â";
				array19[174] = "A";
				array19[175] = "a";
				array19[176] = "A";
				array19[177] = "a";
				array19[178] = "A";
				array19[179] = "a";
				array19[180] = "A";
				array19[181] = "a";
				array19[184] = "E";
				array19[185] = "e";
				array19[186] = "E";
				array19[187] = "e";
				array19[188] = "E";
				array19[189] = "e";
				array19[190] = "Ê";
				array19[191] = "ê";
				array19[192] = "Ê";
				array19[193] = "ê";
				array19[194] = "Ê";
				array19[195] = "ê";
				array19[196] = "Ê";
				array19[197] = "ê";
				array19[200] = "I";
				array19[201] = "i";
				array19[202] = "I";
				array19[203] = "i";
				array19[204] = "O";
				array19[205] = "o";
				array19[206] = "O";
				array19[207] = "o";
				array19[208] = "Ô";
				array19[209] = "ô";
				array19[210] = "Ô";
				array19[211] = "ô";
				array19[212] = "Ô";
				array19[213] = "ô";
				array19[214] = "Ô";
				array19[215] = "ô";
				array19[218] = "O";
				array19[219] = "o";
				array19[220] = "O";
				array19[221] = "o";
				array19[222] = "O";
				array19[223] = "o";
				array19[224] = "O";
				array19[225] = "o";
				array19[226] = "O";
				array19[227] = "o";
				array19[228] = "U";
				array19[229] = "u";
				array19[230] = "U";
				array19[231] = "u";
				array19[232] = "U";
				array19[233] = "u";
				array19[234] = "U";
				array19[235] = "u";
				array19[236] = "U";
				array19[237] = "u";
				array19[238] = "U";
				array19[239] = "u";
				array19[240] = "U";
				array19[241] = "u";
				array19[242] = "Y";
				array19[243] = "y";
				array19[244] = "Y";
				array19[245] = "y";
				array19[246] = "Y";
				array19[247] = "y";
				array19[248] = "Y";
				array19[249] = "y";
				array19[256] = "a";
				array19[257] = "a";
				array19[272] = "e";
				array19[273] = "e";
				array18[num25] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num26, num27, array19);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array20 = array;
				int num28 = 11;
				int num29 = 8032;
				int num30 = 8063;
				string[] array21 = new string[32];
				array21[8] = "O";
				array21[9] = "O";
				array21[16] = "a";
				array21[18] = "e";
				array20[num28] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num29, num30, array21);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array22 = array;
				int num31 = 12;
				int num32 = 8112;
				int num33 = 8143;
				string[] array23 = new string[32];
				array23[0] = "a";
				array23[1] = "a";
				array23[3] = "a";
				array23[6] = "a";
				array23[17] = "¨";
				array22[num31] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num32, num33, array23);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array24 = array;
				int num34 = 13;
				int num35 = 8160;
				int num36 = 8191;
				string[] array25 = new string[32];
				array25[13] = "¨";
				array25[15] = "`";
				array25[26] = "O";
				array25[28] = "O";
				array25[29] = "´";
				array24[num34] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num35, num36, array25);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array26 = array;
				int num37 = 14;
				int num38 = 8208;
				int num39 = 8287;
				string[] array27 = new string[80];
				array27[1] = "-";
				array27[20] = ".";
				array27[21] = "..";
				array27[22] = "...";
				array27[35] = "''";
				array27[36] = "'''";
				array27[38] = "``";
				array27[39] = "```";
				array27[44] = "!!";
				array27[55] = "??";
				array27[56] = "?!";
				array27[57] = "!?";
				array27[71] = "''''";
				array26[num37] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num38, num39, array27);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array28 = array;
				int num40 = 15;
				int num41 = 8304;
				int num42 = 8367;
				string[] array29 = new string[64];
				array29[0] = "0";
				array29[1] = "i";
				array29[4] = "4";
				array29[5] = "5";
				array29[6] = "6";
				array29[7] = "7";
				array29[8] = "8";
				array29[9] = "9";
				array29[10] = "+";
				array29[11] = "-";
				array29[12] = "=";
				array29[13] = "(";
				array29[14] = ")";
				array29[15] = "n";
				array29[16] = "0";
				array29[17] = "1";
				array29[18] = "2";
				array29[19] = "3";
				array29[20] = "4";
				array29[21] = "5";
				array29[22] = "6";
				array29[23] = "7";
				array29[24] = "8";
				array29[25] = "9";
				array29[26] = "+";
				array29[27] = "-";
				array29[28] = "=";
				array29[29] = "(";
				array29[30] = ")";
				array29[32] = "a";
				array29[33] = "e";
				array29[34] = "o";
				array29[35] = "x";
				array29[56] = "Rs";
				array28[num40] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num41, num42, array29);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array30 = array;
				int num43 = 16;
				int num44 = 8448;
				int num45 = 8591;
				string[] array31 = new string[144];
				array31[0] = "a/c";
				array31[1] = "a/s";
				array31[2] = "C";
				array31[3] = "°C";
				array31[5] = "c/o";
				array31[6] = "c/u";
				array31[9] = "°F";
				array31[10] = "g";
				array31[11] = "H";
				array31[12] = "H";
				array31[13] = "H";
				array31[14] = "h";
				array31[15] = "h";
				array31[16] = "I";
				array31[17] = "I";
				array31[18] = "L";
				array31[19] = "l";
				array31[21] = "N";
				array31[22] = "No";
				array31[25] = "P";
				array31[26] = "Q";
				array31[27] = "R";
				array31[28] = "R";
				array31[29] = "R";
				array31[32] = "SM";
				array31[33] = "TEL";
				array31[34] = "TM";
				array31[36] = "Z";
				array31[38] = "O";
				array31[40] = "Z";
				array31[42] = "K";
				array31[43] = "Å";
				array31[44] = "B";
				array31[45] = "C";
				array31[47] = "e";
				array31[48] = "E";
				array31[49] = "F";
				array31[51] = "M";
				array31[52] = "o";
				array31[57] = "i";
				array31[59] = "FAX";
				array31[60] = "p";
				array31[62] = "G";
				array31[69] = "D";
				array31[70] = "d";
				array31[71] = "e";
				array31[72] = "i";
				array31[73] = "j";
				array31[80] = "1/7";
				array31[81] = "1/9";
				array31[82] = "1/10";
				array31[83] = "1/3";
				array31[84] = "2/3";
				array31[85] = "1/5";
				array31[86] = "2/5";
				array31[87] = "3/5";
				array31[88] = "4/5";
				array31[89] = "1/6";
				array31[90] = "5/6";
				array31[91] = "1/8";
				array31[92] = "3/8";
				array31[93] = "5/8";
				array31[94] = "7/8";
				array31[95] = "1/";
				array31[96] = "I";
				array31[97] = "II";
				array31[98] = "III";
				array31[99] = "IV";
				array31[100] = "V";
				array31[101] = "VI";
				array31[102] = "VII";
				array31[103] = "VIII";
				array31[104] = "IX";
				array31[105] = "X";
				array31[106] = "XI";
				array31[107] = "XII";
				array31[108] = "L";
				array31[109] = "C";
				array31[110] = "D";
				array31[111] = "M";
				array31[112] = "i";
				array31[113] = "ii";
				array31[114] = "iii";
				array31[115] = "iv";
				array31[116] = "v";
				array31[117] = "vi";
				array31[118] = "vii";
				array31[119] = "viii";
				array31[120] = "ix";
				array31[121] = "x";
				array31[122] = "xi";
				array31[123] = "xii";
				array31[124] = "l";
				array31[125] = "c";
				array31[126] = "d";
				array31[127] = "m";
				array31[137] = "0/3";
				array30[num43] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num44, num45, array31);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array32 = array;
				int num46 = 17;
				int num47 = 8736;
				int num48 = 8751;
				string[] array33 = new string[16];
				array33[4] = "|";
				array32[num46] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num47, num48, array33);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array34 = array;
				int num49 = 18;
				int num50 = 8768;
				int num51 = 8783;
				string[] array35 = new string[16];
				array35[1] = "~";
				array35[9] = "˜";
				array34[num49] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num50, num51, array35);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array36 = array;
				int num52 = 19;
				int num53 = 8800;
				int num54 = 8831;
				string[] array37 = new string[32];
				array37[0] = "=";
				array37[2] = "=";
				array37[14] = "<";
				array37[15] = ">";
				array37[16] = "=";
				array37[17] = "=";
				array36[num52] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num53, num54, array37);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array38 = array;
				int num55 = 20;
				int num56 = 8992;
				int num57 = 9007;
				string[] array39 = new string[16];
				array39[9] = "<";
				array39[10] = ">";
				array38[num55] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num56, num57, array39);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array40 = array;
				int num58 = 21;
				int num59 = 9312;
				int num60 = 9455;
				string[] array41 = new string[144];
				array41[0] = "1";
				array41[1] = "2";
				array41[2] = "3";
				array41[3] = "4";
				array41[4] = "5";
				array41[5] = "6";
				array41[6] = "7";
				array41[7] = "8";
				array41[8] = "9";
				array41[9] = "10";
				array41[10] = "11";
				array41[11] = "12";
				array41[12] = "13";
				array41[13] = "14";
				array41[14] = "15";
				array41[15] = "16";
				array41[16] = "17";
				array41[17] = "18";
				array41[18] = "19";
				array41[19] = "20";
				array41[20] = "(1)";
				array41[21] = "(2)";
				array41[22] = "(3)";
				array41[23] = "(4)";
				array41[24] = "(5)";
				array41[25] = "(6)";
				array41[26] = "(7)";
				array41[27] = "(8)";
				array41[28] = "(9)";
				array41[29] = "(10)";
				array41[30] = "(11)";
				array41[31] = "(12)";
				array41[32] = "(13)";
				array41[33] = "(14)";
				array41[34] = "(15)";
				array41[35] = "(16)";
				array41[36] = "(17)";
				array41[37] = "(18)";
				array41[38] = "(19)";
				array41[39] = "(20)";
				array41[40] = "1.";
				array41[41] = "2.";
				array41[42] = "3.";
				array41[43] = "4.";
				array41[44] = "5.";
				array41[45] = "6.";
				array41[46] = "7.";
				array41[47] = "8.";
				array41[48] = "9.";
				array41[49] = "10.";
				array41[50] = "11.";
				array41[51] = "12.";
				array41[52] = "13.";
				array41[53] = "14.";
				array41[54] = "15.";
				array41[55] = "16.";
				array41[56] = "17.";
				array41[57] = "18.";
				array41[58] = "19.";
				array41[59] = "20.";
				array41[60] = "(a)";
				array41[61] = "(b)";
				array41[62] = "(c)";
				array41[63] = "(d)";
				array41[64] = "(e)";
				array41[65] = "(f)";
				array41[66] = "(g)";
				array41[67] = "(h)";
				array41[68] = "(i)";
				array41[69] = "(j)";
				array41[70] = "(k)";
				array41[71] = "(l)";
				array41[72] = "(m)";
				array41[73] = "(n)";
				array41[74] = "(o)";
				array41[75] = "(p)";
				array41[76] = "(q)";
				array41[77] = "(r)";
				array41[78] = "(s)";
				array41[79] = "(t)";
				array41[80] = "(u)";
				array41[81] = "(v)";
				array41[82] = "(w)";
				array41[83] = "(x)";
				array41[84] = "(y)";
				array41[85] = "(z)";
				array41[86] = "A";
				array41[87] = "B";
				array41[88] = "C";
				array41[89] = "D";
				array41[90] = "E";
				array41[91] = "F";
				array41[92] = "G";
				array41[93] = "H";
				array41[94] = "I";
				array41[95] = "J";
				array41[96] = "K";
				array41[97] = "L";
				array41[98] = "M";
				array41[99] = "N";
				array41[100] = "O";
				array41[101] = "P";
				array41[102] = "Q";
				array41[103] = "R";
				array41[104] = "S";
				array41[105] = "T";
				array41[106] = "U";
				array41[107] = "V";
				array41[108] = "W";
				array41[109] = "X";
				array41[110] = "Y";
				array41[111] = "Z";
				array41[112] = "a";
				array41[113] = "b";
				array41[114] = "c";
				array41[115] = "d";
				array41[116] = "e";
				array41[117] = "f";
				array41[118] = "g";
				array41[119] = "h";
				array41[120] = "i";
				array41[121] = "j";
				array41[122] = "k";
				array41[123] = "l";
				array41[124] = "m";
				array41[125] = "n";
				array41[126] = "o";
				array41[127] = "p";
				array41[128] = "q";
				array41[129] = "r";
				array41[130] = "s";
				array41[131] = "t";
				array41[132] = "u";
				array41[133] = "v";
				array41[134] = "w";
				array41[135] = "x";
				array41[136] = "y";
				array41[137] = "z";
				array41[138] = "0";
				array40[num58] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num59, num60, array41);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array42 = array;
				int num61 = 22;
				int num62 = 10864;
				int num63 = 10879;
				string[] array43 = new string[16];
				array43[4] = "::=";
				array43[5] = "==";
				array43[6] = "===";
				array42[num61] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num62, num63, array43);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array44 = array;
				int num64 = 23;
				int num65 = 11376;
				int num66 = 11391;
				string[] array45 = new string[16];
				array45[12] = "j";
				array45[13] = "V";
				array44[num64] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num65, num66, array45);
				array[24] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(12880, 12895, new string[]
				{
					"PTE",
					"21",
					"22",
					"23",
					"24",
					"25",
					"26",
					"27",
					"28",
					"29",
					"30",
					"31",
					"32",
					"33",
					"34",
					"35"
				});
				array[25] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(12976, 13007, new string[]
				{
					null,
					"36",
					"37",
					"38",
					"39",
					"40",
					"41",
					"42",
					"43",
					"44",
					"45",
					"46",
					"47",
					"48",
					"49",
					"50",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"Hg",
					"erg",
					"eV",
					"LTD"
				});
				array[26] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(13168, 13279, new string[]
				{
					null,
					"hPa",
					"da",
					"AU",
					"bar",
					"oV",
					"pc",
					"dm",
					"dm²",
					"dm³",
					"IU",
					null,
					null,
					null,
					null,
					null,
					"pA",
					"nA",
					"µA",
					"mA",
					"kA",
					"KB",
					"MB",
					"GB",
					"cal",
					"kcal",
					"pF",
					"nF",
					"µF",
					"µg",
					"mg",
					"kg",
					"Hz",
					"kHz",
					"MHz",
					"GHz",
					"THz",
					"µl",
					"ml",
					"dl",
					"kl",
					"fm",
					"nm",
					"µm",
					"mm",
					"cm",
					"km",
					"mm²",
					"cm²",
					"m²",
					"km²",
					"mm³",
					"cm³",
					"m³",
					"km³",
					"m/s",
					"m/s²",
					"Pa",
					"kPa",
					"MPa",
					"GPa",
					"rad",
					"rad/s",
					"rad/s²",
					"ps",
					"ns",
					"µs",
					"ms",
					"pV",
					"nV",
					"µV",
					"mV",
					"kV",
					"MV",
					"pW",
					"nW",
					"µW",
					"mW",
					"kW",
					"MW",
					"kO",
					"MO",
					"a.m.",
					"Bq",
					"cc",
					"cd",
					"C/kg",
					"Co.",
					"dB",
					"Gy",
					"ha",
					"HP",
					"in",
					"KK",
					"KM",
					"kt",
					"lm",
					"ln",
					"log",
					"lx",
					"mb",
					"mil",
					"mol",
					"PH",
					"p.m.",
					"PPM",
					"PR",
					"sr",
					"Sv",
					"Wb",
					"V/m",
					"A/m"
				});
				array[27] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(13296, 13311, new string[]
				{
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"gal"
				});
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array46 = array;
				int num67 = 28;
				int num68 = 64256;
				int num69 = 64271;
				string[] array47 = new string[16];
				array47[0] = "ff";
				array47[1] = "fi";
				array47[2] = "fl";
				array47[3] = "ffi";
				array47[4] = "ffl";
				array47[6] = "st";
				array46[num67] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num68, num69, array47);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array48 = array;
				int num70 = 29;
				int num71 = 64288;
				int num72 = 64303;
				string[] array49 = new string[16];
				array49[9] = "+";
				array48[num70] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num71, num72, array49);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array50 = array;
				int num73 = 30;
				int num74 = 65040;
				int num75 = 65055;
				string[] array51 = new string[16];
				array51[0] = ",";
				array51[3] = ":";
				array51[4] = ";";
				array51[5] = "!";
				array51[6] = "?";
				array51[9] = "…";
				array50[num73] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num74, num75, array51);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array52 = array;
				int num76 = 31;
				int num77 = 65072;
				int num78 = 65135;
				string[] array53 = new string[64];
				array53[1] = "—";
				array53[2] = "–";
				array53[3] = "_";
				array53[4] = "_";
				array53[5] = "(";
				array53[6] = ")";
				array53[7] = "{";
				array53[8] = "}";
				array53[13] = "«";
				array53[14] = "»";
				array53[15] = "<";
				array53[16] = ">";
				array53[23] = "[";
				array53[24] = "]";
				array53[29] = "_";
				array53[30] = "_";
				array53[31] = "_";
				array53[32] = ",";
				array53[34] = ".";
				array53[36] = ";";
				array53[37] = ":";
				array53[38] = "?";
				array53[39] = "!";
				array53[40] = "—";
				array53[41] = "(";
				array53[42] = ")";
				array53[43] = "{";
				array53[44] = "}";
				array53[47] = "#";
				array53[48] = "&";
				array53[49] = "*";
				array53[50] = "+";
				array53[51] = "-";
				array53[52] = "<";
				array53[53] = ">";
				array53[54] = "=";
				array53[56] = "\\";
				array53[57] = "$";
				array53[58] = "%";
				array53[59] = "@";
				array52[num76] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num77, num78, array53);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array54 = array;
				int num79 = 32;
				int num80 = 65280;
				int num81 = 65391;
				string[] array55 = new string[112];
				array55[1] = "!";
				array55[2] = "\"";
				array55[3] = "#";
				array55[4] = "$";
				array55[5] = "%";
				array55[6] = "&";
				array55[7] = "'";
				array55[8] = "(";
				array55[9] = ")";
				array55[10] = "*";
				array55[11] = "+";
				array55[12] = ",";
				array55[13] = "-";
				array55[14] = ".";
				array55[15] = "/";
				array55[16] = "0";
				array55[17] = "1";
				array55[18] = "2";
				array55[19] = "3";
				array55[20] = "4";
				array55[21] = "5";
				array55[22] = "6";
				array55[23] = "7";
				array55[24] = "8";
				array55[25] = "9";
				array55[26] = ":";
				array55[27] = ";";
				array55[28] = "<";
				array55[29] = "=";
				array55[30] = ">";
				array55[31] = "?";
				array55[32] = "@";
				array55[33] = "A";
				array55[34] = "B";
				array55[35] = "C";
				array55[36] = "D";
				array55[37] = "E";
				array55[38] = "F";
				array55[39] = "G";
				array55[40] = "H";
				array55[41] = "I";
				array55[42] = "J";
				array55[43] = "K";
				array55[44] = "L";
				array55[45] = "M";
				array55[46] = "N";
				array55[47] = "O";
				array55[48] = "P";
				array55[49] = "Q";
				array55[50] = "R";
				array55[51] = "S";
				array55[52] = "T";
				array55[53] = "U";
				array55[54] = "V";
				array55[55] = "W";
				array55[56] = "X";
				array55[57] = "Y";
				array55[58] = "Z";
				array55[59] = "[";
				array55[60] = "\\";
				array55[61] = "]";
				array55[62] = "^";
				array55[63] = "_";
				array55[64] = "`";
				array55[65] = "a";
				array55[66] = "b";
				array55[67] = "c";
				array55[68] = "d";
				array55[69] = "e";
				array55[70] = "f";
				array55[71] = "g";
				array55[72] = "h";
				array55[73] = "i";
				array55[74] = "j";
				array55[75] = "k";
				array55[76] = "l";
				array55[77] = "m";
				array55[78] = "n";
				array55[79] = "o";
				array55[80] = "p";
				array55[81] = "q";
				array55[82] = "r";
				array55[83] = "s";
				array55[84] = "t";
				array55[85] = "u";
				array55[86] = "v";
				array55[87] = "w";
				array55[88] = "x";
				array55[89] = "y";
				array55[90] = "z";
				array55[91] = "{";
				array55[92] = "|";
				array55[93] = "}";
				array55[94] = "~";
				array55[101] = "·";
				array54[num79] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num80, num81, array55);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array56 = array;
				int num82 = 33;
				int num83 = 65504;
				int num84 = 65519;
				string[] array57 = new string[16];
				array57[0] = "¢";
				array57[1] = "£";
				array57[2] = "¬";
				array57[3] = "¯";
				array57[4] = "¦";
				array57[5] = "¥";
				array57[8] = "¦";
				array57[13] = "¦";
				array56[num82] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num83, num84, array57);
				array[34] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(119808, 120831, new string[]
				{
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					null,
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					null,
					"C",
					"D",
					null,
					null,
					"G",
					null,
					null,
					"J",
					"K",
					null,
					null,
					"N",
					"O",
					"P",
					"Q",
					null,
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					null,
					"f",
					null,
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					null,
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					null,
					"D",
					"E",
					"F",
					"G",
					null,
					null,
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					null,
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					null,
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					null,
					"D",
					"E",
					"F",
					"G",
					null,
					"I",
					"J",
					"K",
					"L",
					"M",
					null,
					"O",
					null,
					null,
					null,
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					null,
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"A",
					"B",
					"C",
					"D",
					"E",
					"F",
					"G",
					"H",
					"I",
					"J",
					"K",
					"L",
					"M",
					"N",
					"O",
					"P",
					"Q",
					"R",
					"S",
					"T",
					"U",
					"V",
					"W",
					"X",
					"Y",
					"Z",
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"i",
					"j",
					"k",
					"l",
					"m",
					"n",
					"o",
					"p",
					"q",
					"r",
					"s",
					"t",
					"u",
					"v",
					"w",
					"x",
					"y",
					"z",
					"i",
					null,
					null,
					null,
					null,
					null,
					"G",
					null,
					null,
					null,
					null,
					"T",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"S",
					null,
					null,
					"F",
					null,
					null,
					"O",
					null,
					"a",
					"ß",
					null,
					"d",
					"e",
					null,
					null,
					null,
					null,
					null,
					null,
					"µ",
					null,
					null,
					null,
					"p",
					null,
					null,
					"s",
					"t",
					null,
					"f",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"G",
					null,
					null,
					null,
					null,
					"T",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"S",
					null,
					null,
					"F",
					null,
					null,
					"O",
					null,
					"a",
					"ß",
					null,
					"d",
					"e",
					null,
					null,
					null,
					null,
					null,
					null,
					"µ",
					null,
					null,
					null,
					"p",
					null,
					null,
					"s",
					"t",
					null,
					"f",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"G",
					null,
					null,
					null,
					null,
					"T",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"S",
					null,
					null,
					"F",
					null,
					null,
					"O",
					null,
					"a",
					"ß",
					null,
					"d",
					"e",
					null,
					null,
					null,
					null,
					null,
					null,
					"µ",
					null,
					null,
					null,
					"p",
					null,
					null,
					"s",
					"t",
					null,
					"f",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"G",
					null,
					null,
					null,
					null,
					"T",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"S",
					null,
					null,
					"F",
					null,
					null,
					"O",
					null,
					"a",
					"ß",
					null,
					"d",
					"e",
					null,
					null,
					null,
					null,
					null,
					null,
					"µ",
					null,
					null,
					null,
					"p",
					null,
					null,
					"s",
					"t",
					null,
					"f",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"G",
					null,
					null,
					null,
					null,
					"T",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"S",
					null,
					null,
					"F",
					null,
					null,
					"O",
					null,
					"a",
					"ß",
					null,
					"d",
					"e",
					null,
					null,
					null,
					null,
					null,
					null,
					"µ",
					null,
					null,
					null,
					"p",
					null,
					null,
					"s",
					"t",
					null,
					"f",
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					"0",
					"1",
					"2",
					"3",
					"4",
					"5",
					"6",
					"7",
					"8",
					"9",
					"0",
					"1",
					"2",
					"3",
					"4",
					"5",
					"6",
					"7",
					"8",
					"9",
					"0",
					"1",
					"2",
					"3",
					"4",
					"5",
					"6",
					"7",
					"8",
					"9",
					"0",
					"1",
					"2",
					"3",
					"4",
					"5",
					"6",
					"7",
					"8",
					"9",
					"0",
					"1",
					"2",
					"3",
					"4",
					"5",
					"6",
					"7",
					"8",
					"9"
				});
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array58 = array;
				int num85 = 35;
				int num86 = 127232;
				int num87 = 127311;
				string[] array59 = new string[80];
				array59[0] = "0.";
				array59[1] = "0,";
				array59[2] = "1,";
				array59[3] = "2,";
				array59[4] = "3,";
				array59[5] = "4,";
				array59[6] = "5,";
				array59[7] = "6,";
				array59[8] = "7,";
				array59[9] = "8,";
				array59[10] = "9,";
				array59[16] = "(A)";
				array59[17] = "(B)";
				array59[18] = "(C)";
				array59[19] = "(D)";
				array59[20] = "(E)";
				array59[21] = "(F)";
				array59[22] = "(G)";
				array59[23] = "(H)";
				array59[24] = "(I)";
				array59[25] = "(J)";
				array59[26] = "(K)";
				array59[27] = "(L)";
				array59[28] = "(M)";
				array59[29] = "(N)";
				array59[30] = "(O)";
				array59[31] = "(P)";
				array59[32] = "(Q)";
				array59[33] = "(R)";
				array59[34] = "(S)";
				array59[35] = "(T)";
				array59[36] = "(U)";
				array59[37] = "(V)";
				array59[38] = "(W)";
				array59[39] = "(X)";
				array59[40] = "(Y)";
				array59[41] = "(Z)";
				array59[43] = "C";
				array59[44] = "R";
				array59[45] = "CD";
				array59[46] = "WZ";
				array59[49] = "B";
				array59[61] = "N";
				array59[63] = "P";
				array59[66] = "S";
				array59[70] = "W";
				array59[74] = "HV";
				array59[75] = "MV";
				array59[76] = "SD";
				array59[77] = "SS";
				array59[78] = "PPV";
				array58[num85] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num86, num87, array59);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array60 = array;
				int num88 = 36;
				int num89 = 127376;
				int num90 = 127391;
				string[] array61 = new string[16];
				array61[0] = "DJ";
				array60[num88] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num89, num90, array61);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] array62 = array;
				int num91 = 37;
				int num92 = 194560;
				int num93 = 194575;
				string[] array63 = new string[16];
				array63[3] = "G";
				array62[num91] = new UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper(num92, num93, array63);
				UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper.MappingTable = array;
			}

			public const int MaxFallbackCharCount = 6;

			private static readonly UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper[] MappingTable;

			private int minCodePoint;

			private int maxCodePoint;

			private string[] mapping;
		}

		private class EncoderAnsiBestFitFallback : EncoderFallback
		{
			public EncoderAnsiBestFitFallback(bool skipInvalidCharacter, char invalidCharacterReplacement)
			{
				this.internalBestFitFallback = UnicodeToAnsiConverter.DefaultAnsiEncoding.EncoderFallback;
				this.skipInvalidCharacter = skipInvalidCharacter;
				this.invalidCharacterReplacement = invalidCharacterReplacement;
			}

			public override EncoderFallbackBuffer CreateFallbackBuffer()
			{
				return new UnicodeToAnsiConverter.EncoderAnsiBestFitFallbackFallbackBuffer(this);
			}

			public override int MaxCharCount
			{
				get
				{
					return 6;
				}
			}

			internal EncoderFallback InternalBestFitFallback
			{
				get
				{
					return this.internalBestFitFallback;
				}
			}

			internal bool SkipInvalidCharacter
			{
				get
				{
					return this.skipInvalidCharacter;
				}
			}

			internal char InvalidCharacterReplacement
			{
				get
				{
					return this.invalidCharacterReplacement;
				}
			}

			private EncoderFallback internalBestFitFallback;

			private bool skipInvalidCharacter;

			private char invalidCharacterReplacement;
		}

		private class EncoderAnsiBestFitFallbackFallbackBuffer : EncoderFallbackBuffer
		{
			public EncoderAnsiBestFitFallbackFallbackBuffer(UnicodeToAnsiConverter.EncoderAnsiBestFitFallback fallback)
			{
				this.fallback = fallback;
				this.internalBestFitFallbackBuffer = fallback.InternalBestFitFallback.CreateFallbackBuffer();
			}

			public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
			{
				this.defaultMechanism = true;
				this.buffer = new char[]
				{
					this.fallback.InvalidCharacterReplacement
				};
				this.index = 0;
				return true;
			}

			public override bool Fallback(char charUnknown, int index)
			{
				if (this.internalBestFitFallbackBuffer.Fallback(charUnknown, index))
				{
					this.buffer = new char[this.internalBestFitFallbackBuffer.Remaining];
					bool flag = true;
					int num = 0;
					while (this.buffer.Length > num)
					{
						this.buffer[num] = this.internalBestFitFallbackBuffer.GetNextChar();
						if ('?' != this.buffer[num])
						{
							flag = false;
						}
						num++;
					}
					if (flag)
					{
						this.buffer = null;
					}
				}
				if (this.buffer == null)
				{
					string text = UnicodeToAnsiConverter.UnicodeToAnsiBestFitMapper.Map((int)charUnknown);
					if (text == null)
					{
						this.defaultMechanism = true;
						this.buffer = new char[]
						{
							this.fallback.InvalidCharacterReplacement
						};
					}
					else
					{
						this.buffer = text.ToCharArray();
					}
				}
				this.index = 0;
				return true;
			}

			public override char GetNextChar()
			{
				if (this.buffer.Length == this.index || (this.defaultMechanism && this.fallback.SkipInvalidCharacter))
				{
					return '\0';
				}
				return this.buffer[this.index++];
			}

			public override bool MovePrevious()
			{
				if (0 < this.index)
				{
					this.index--;
					return true;
				}
				return false;
			}

			public override int Remaining
			{
				get
				{
					if (this.defaultMechanism && this.fallback.SkipInvalidCharacter)
					{
						return 0;
					}
					return this.buffer.Length - this.index;
				}
			}

			public override void Reset()
			{
				base.Reset();
				this.buffer = null;
				this.index = 0;
				this.defaultMechanism = false;
			}

			private UnicodeToAnsiConverter.EncoderAnsiBestFitFallback fallback;

			private EncoderFallbackBuffer internalBestFitFallbackBuffer;

			private char[] buffer;

			private int index;

			private bool defaultMechanism;
		}
	}
}
