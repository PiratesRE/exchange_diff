using System;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AliasHelper
	{
		private static AliasHelper.CharacterCategory Analyze(char c)
		{
			if (AliasHelper.CharactersCategoryMap.Length > (int)c)
			{
				return AliasHelper.CharactersCategoryMap[(int)c];
			}
			return AliasHelper.CharacterCategory.InvalidCharacter;
		}

		public static string GenerateAlias(string input)
		{
			return AliasHelper.GenerateAlias(input, AliasHelper.MaximalAliasLength, true, false, '?');
		}

		public static string GenerateAlias(string input, bool skipInvalidCharacter)
		{
			return AliasHelper.GenerateAlias(input, AliasHelper.MaximalAliasLength, true, skipInvalidCharacter, '?');
		}

		public static string GenerateAlias(string input, char invalidCharacterReplacement)
		{
			return AliasHelper.GenerateAlias(input, AliasHelper.MaximalAliasLength, true, false, invalidCharacterReplacement);
		}

		private static string GenerateAlias(string input, int desiredLength, bool mapToAnsi, bool skipInvalidCharacter, char invalidCharacterReplacement)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException("input");
			}
			if (AliasHelper.Analyze(invalidCharacterReplacement) != AliasHelper.CharacterCategory.RegularCharacter)
			{
				throw new ArgumentOutOfRangeException("invalidCharacterReplacement", invalidCharacterReplacement, new LocalizedString("DEV BUG: Parameter 'invalidCharacterReplacement' should be category of CharacterCategory.RegularCharacter."));
			}
			if (mapToAnsi)
			{
				input = UnicodeToAnsiConverter.Convert(input, skipInvalidCharacter, invalidCharacterReplacement);
			}
			StringBuilder stringBuilder = new StringBuilder(input.Length);
			AliasHelper.RecognisingState recognisingState = AliasHelper.RecognisingState.Start;
			int num = 0;
			int num2 = 0;
			while (input.Length > num && desiredLength > num2)
			{
				char c = input[num];
				AliasHelper.CharacterCategory characterCategory = AliasHelper.Analyze(c);
				if (characterCategory != AliasHelper.CharacterCategory.WhiteSpaceSymbol)
				{
					switch (recognisingState)
					{
					case AliasHelper.RecognisingState.Start:
						switch (characterCategory)
						{
						case AliasHelper.CharacterCategory.RegularCharacter:
							recognisingState = AliasHelper.RecognisingState.Terminal;
							break;
						case AliasHelper.CharacterCategory.InvalidCharacter:
						case AliasHelper.CharacterCategory.DotSymbol:
							if (skipInvalidCharacter)
							{
								goto IL_149;
							}
							c = invalidCharacterReplacement;
							recognisingState = AliasHelper.RecognisingState.Terminal;
							break;
						}
						break;
					case AliasHelper.RecognisingState.Terminal:
						switch (characterCategory)
						{
						case AliasHelper.CharacterCategory.InvalidCharacter:
							if (skipInvalidCharacter)
							{
								goto IL_149;
							}
							c = invalidCharacterReplacement;
							break;
						case AliasHelper.CharacterCategory.DotSymbol:
							if (input.Length - 1 == num || desiredLength - 1 == num2)
							{
								if (skipInvalidCharacter)
								{
									goto IL_149;
								}
								c = invalidCharacterReplacement;
							}
							else
							{
								recognisingState = AliasHelper.RecognisingState.Middle;
							}
							break;
						}
						break;
					case AliasHelper.RecognisingState.Middle:
						switch (characterCategory)
						{
						case AliasHelper.CharacterCategory.RegularCharacter:
							recognisingState = AliasHelper.RecognisingState.Terminal;
							break;
						case AliasHelper.CharacterCategory.InvalidCharacter:
						case AliasHelper.CharacterCategory.DotSymbol:
							if (skipInvalidCharacter && input.Length - 1 > num && desiredLength - 1 > num2)
							{
								goto IL_149;
							}
							c = invalidCharacterReplacement;
							recognisingState = AliasHelper.RecognisingState.Terminal;
							break;
						}
						break;
					}
					stringBuilder.Append(c);
					num2++;
				}
				IL_149:
				num++;
			}
			if (AliasHelper.RecognisingState.Terminal != recognisingState)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AliasHelper()
		{
			AliasHelper.CharacterCategory[] array = new AliasHelper.CharacterCategory[256];
			array[0] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[1] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[2] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[3] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[4] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[5] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[6] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[7] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[8] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[9] = AliasHelper.CharacterCategory.WhiteSpaceSymbol;
			array[10] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[11] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[12] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[13] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[14] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[15] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[16] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[17] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[18] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[19] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[20] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[21] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[22] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[23] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[24] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[25] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[26] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[27] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[28] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[29] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[30] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[31] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[32] = AliasHelper.CharacterCategory.WhiteSpaceSymbol;
			array[34] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[40] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[41] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[44] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[46] = AliasHelper.CharacterCategory.DotSymbol;
			array[58] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[59] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[60] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[62] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[64] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[91] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[92] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[93] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[127] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[128] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[129] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[130] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[131] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[132] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[133] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[134] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[135] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[136] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[137] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[138] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[139] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[140] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[141] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[142] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[143] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[144] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[145] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[146] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[147] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[148] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[149] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[150] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[151] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[152] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[153] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[154] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[155] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[156] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[157] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[158] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[159] = AliasHelper.CharacterCategory.InvalidCharacter;
			array[160] = AliasHelper.CharacterCategory.InvalidCharacter;
			AliasHelper.CharactersCategoryMap = array;
		}

		private const char DefaultInvalidCharacterReplacement = '?';

		public static readonly int MaximalAliasLength = 64;

		private static readonly AliasHelper.CharacterCategory[] CharactersCategoryMap;

		private enum CharacterCategory
		{
			RegularCharacter,
			InvalidCharacter,
			DotSymbol,
			WhiteSpaceSymbol
		}

		private enum RecognisingState
		{
			Start,
			Terminal,
			Middle
		}
	}
}
