using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal class DtmfString
	{
		static DtmfString()
		{
			DtmfString.normalizationTable['Ɓ'] = 'B';
			DtmfString.normalizationTable['Ƃ'] = 'B';
			DtmfString.normalizationTable['Ƈ'] = 'C';
			DtmfString.normalizationTable['Đ'] = 'D';
			DtmfString.normalizationTable['Ɗ'] = 'D';
			DtmfString.normalizationTable['Ƌ'] = 'D';
			DtmfString.normalizationTable['Ƒ'] = 'F';
			DtmfString.normalizationTable['Ɠ'] = 'G';
			DtmfString.normalizationTable['Ǥ'] = 'G';
			DtmfString.normalizationTable['Ħ'] = 'H';
			DtmfString.normalizationTable['Ɨ'] = 'I';
			DtmfString.normalizationTable['Ƙ'] = 'K';
			DtmfString.normalizationTable['Ł'] = 'L';
			DtmfString.normalizationTable['Ɲ'] = 'N';
			DtmfString.normalizationTable['Ƞ'] = 'N';
			DtmfString.normalizationTable['Ø'] = 'O';
			DtmfString.normalizationTable['Ɵ'] = 'O';
			DtmfString.normalizationTable['Ǿ'] = 'O';
			DtmfString.normalizationTable['Ƥ'] = 'P';
			DtmfString.normalizationTable['Ŧ'] = 'T';
			DtmfString.normalizationTable['Ƭ'] = 'T';
			DtmfString.normalizationTable['Ʈ'] = 'T';
			DtmfString.normalizationTable['Ʋ'] = 'V';
			DtmfString.normalizationTable['Ƴ'] = 'Y';
			DtmfString.normalizationTable['Ƶ'] = 'Z';
			DtmfString.normalizationTable['Ȥ'] = 'Z';
			DtmfString.normalizationTable['ẚ'] = 'a';
			DtmfString.normalizationTable['ƀ'] = 'b';
			DtmfString.normalizationTable['ƃ'] = 'b';
			DtmfString.normalizationTable['ɓ'] = 'b';
			DtmfString.normalizationTable['ƈ'] = 'c';
			DtmfString.normalizationTable['ɕ'] = 'c';
			DtmfString.normalizationTable['đ'] = 'd';
			DtmfString.normalizationTable['ƌ'] = 'd';
			DtmfString.normalizationTable['ȡ'] = 'd';
			DtmfString.normalizationTable['ɖ'] = 'd';
			DtmfString.normalizationTable['ɗ'] = 'd';
			DtmfString.normalizationTable['ƒ'] = 'f';
			DtmfString.normalizationTable['ǥ'] = 'g';
			DtmfString.normalizationTable['ɠ'] = 'g';
			DtmfString.normalizationTable['ħ'] = 'h';
			DtmfString.normalizationTable['ɦ'] = 'h';
			DtmfString.normalizationTable['ɨ'] = 'i';
			DtmfString.normalizationTable['ʝ'] = 'j';
			DtmfString.normalizationTable['ƙ'] = 'k';
			DtmfString.normalizationTable['ŀ'] = 'l';
			DtmfString.normalizationTable['ł'] = 'l';
			DtmfString.normalizationTable['ƚ'] = 'l';
			DtmfString.normalizationTable['ȴ'] = 'l';
			DtmfString.normalizationTable['ɫ'] = 'l';
			DtmfString.normalizationTable['ɬ'] = 'l';
			DtmfString.normalizationTable['ɭ'] = 'l';
			DtmfString.normalizationTable['ɱ'] = 'm';
			DtmfString.normalizationTable['ƞ'] = 'n';
			DtmfString.normalizationTable['ȵ'] = 'n';
			DtmfString.normalizationTable['ɲ'] = 'n';
			DtmfString.normalizationTable['ɳ'] = 'n';
			DtmfString.normalizationTable['ø'] = 'o';
			DtmfString.normalizationTable['ǿ'] = 'o';
			DtmfString.normalizationTable['ƥ'] = 'p';
			DtmfString.normalizationTable['ʠ'] = 'q';
			DtmfString.normalizationTable['ɼ'] = 'r';
			DtmfString.normalizationTable['ɽ'] = 'r';
			DtmfString.normalizationTable['ʂ'] = 's';
			DtmfString.normalizationTable['ŧ'] = 't';
			DtmfString.normalizationTable['ƫ'] = 't';
			DtmfString.normalizationTable['ƭ'] = 't';
			DtmfString.normalizationTable['ȶ'] = 't';
			DtmfString.normalizationTable['ʈ'] = 't';
			DtmfString.normalizationTable['ʋ'] = 'v';
			DtmfString.normalizationTable['ƴ'] = 'y';
			DtmfString.normalizationTable['ƶ'] = 'z';
			DtmfString.normalizationTable['ȥ'] = 'z';
			DtmfString.normalizationTable['ʐ'] = 'z';
			DtmfString.normalizationTable['ʑ'] = 'z';
		}

		internal static string Reverse(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			char[] array = new char[input.Length];
			for (int i = input.Length - 1; i >= 0; i--)
			{
				array[input.Length - 1 - i] = input[i];
			}
			return new string(array);
		}

		internal static string SanitizePhoneNumber(string rawPhone)
		{
			if (string.IsNullOrEmpty(rawPhone))
			{
				return string.Empty;
			}
			rawPhone = rawPhone.Trim();
			if (string.IsNullOrEmpty(rawPhone))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(rawPhone.Length);
			if ('(' != rawPhone[0])
			{
				if ('+' == rawPhone[0])
				{
					stringBuilder.Append(rawPhone[0]);
				}
				else
				{
					if (!char.IsDigit(rawPhone[0]))
					{
						return string.Empty;
					}
					stringBuilder.Append(char.GetNumericValue(rawPhone[0]));
				}
			}
			for (int i = 1; i < rawPhone.Length; i++)
			{
				char c = rawPhone[i];
				if (char.IsDigit(c))
				{
					stringBuilder.Append(char.GetNumericValue(c));
				}
				else if (!char.IsPunctuation(c) && !char.IsSeparator(c) && !char.IsSymbol(c) && !char.IsWhiteSpace(c))
				{
					break;
				}
			}
			return stringBuilder.ToString();
		}

		internal static string DtmfEncode(string s)
		{
			string text = DtmfString.DtmfNormalize(s);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in text)
			{
				char value;
				if (DtmfString.GetDtmf(c, out value))
				{
					stringBuilder.Append(value);
				}
			}
			return stringBuilder.ToString();
		}

		internal static bool GetDtmf(char c, out char code)
		{
			code = c;
			c = char.ToLower(c);
			if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z'))
			{
				if (c >= '0' && c <= '9')
				{
					code = c;
				}
				if (c >= 'a' && c <= 'c')
				{
					code = '2';
				}
				if (c >= 'd' && c <= 'f')
				{
					code = '3';
				}
				if (c >= 'g' && c <= 'i')
				{
					code = '4';
				}
				if (c >= 'j' && c <= 'l')
				{
					code = '5';
				}
				if (c >= 'm' && c <= 'o')
				{
					code = '6';
				}
				if (c >= 'p' && c <= 's')
				{
					code = '7';
				}
				if (c >= 't' && c <= 'v')
				{
					code = '8';
				}
				if (c >= 'w' && c <= 'z')
				{
					code = '9';
				}
				return true;
			}
			return false;
		}

		internal static string DtmfNormalize(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder(s.Length);
			foreach (char c in s)
			{
				if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
				{
					stringBuilder.Append(c);
				}
			}
			string text = DtmfString.FoldString(stringBuilder.ToString());
			string text2 = text.Normalize(NormalizationForm.FormD);
			stringBuilder.Length = 0;
			foreach (char c2 in text2)
			{
				UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c2);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(DtmfString.NormalizeUsingTable(c2));
				}
			}
			return stringBuilder.ToString();
		}

		private static char NormalizeUsingTable(char input)
		{
			char result;
			if (!DtmfString.normalizationTable.TryGetValue(input, out result))
			{
				return input;
			}
			return result;
		}

		private static string FoldString(string input)
		{
			int num = DtmfString.FoldString(DtmfString.MapFlags.MAP_EXPAND_LIGATURES, input, -1, null, 0);
			if (num == 0)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			if (DtmfString.FoldString(DtmfString.MapFlags.MAP_EXPAND_LIGATURES, input, -1, stringBuilder, num) == 0)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			return stringBuilder.ToString();
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern int FoldString([In] DtmfString.MapFlags flags, [In] string inputString, [In] int inputStringLength, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder outputString, [In] int outputStringLength);

		private static Dictionary<char, char> normalizationTable = new Dictionary<char, char>(75);

		[Flags]
		private enum MapFlags
		{
			MAP_FOLDCZONE = 16,
			MAP_PRECOMPOSED = 32,
			MAP_COMPOSITE = 64,
			MAP_FOLDDIGITS = 128,
			MAP_EXPAND_LIGATURES = 8192
		}
	}
}
