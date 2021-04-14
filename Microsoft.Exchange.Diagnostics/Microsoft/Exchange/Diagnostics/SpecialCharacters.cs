using System;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class SpecialCharacters
	{
		public static bool IsValidKey(string key)
		{
			foreach (char ch in key)
			{
				if (!SpecialCharacters.IsValidKeyChar(ch))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsValidKeyChar(char ch)
		{
			return char.IsLetterOrDigit(ch) || SpecialCharacters.allowedSpecialCharacters.Contains(ch);
		}

		public static string SanitizeForLogging(string key)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty, key.Length);
			foreach (char c in key)
			{
				if (!char.IsLetterOrDigit(c) && !SpecialCharacters.allowedSpecialCharacters.Contains(c))
				{
					stringBuilder.Append('-');
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static readonly byte ColonByte = 58;

		public static readonly byte EqualsByte = 61;

		private static char[] allowedSpecialCharacters = new char[]
		{
			'.',
			'-',
			'[',
			']'
		};
	}
}
