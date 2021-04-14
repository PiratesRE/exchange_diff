using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ADObjectNameHelper
	{
		internal static bool CheckIsUnicodeStringWellFormed(string chars, out int position)
		{
			for (int i = 0; i < chars.Length; i++)
			{
				char c = chars[i];
				position = i;
				if (c == '￾' || c == '￿')
				{
					return false;
				}
				if (char.IsHighSurrogate(c))
				{
					if (i + 1 >= chars.Length || !char.IsLowSurrogate(chars[i + 1]))
					{
						return false;
					}
					i++;
				}
				else if (char.IsLowSurrogate(c))
				{
					return false;
				}
			}
			position = -1;
			return true;
		}

		private static string reservedStringPattern = "^[^\\x0a\\x00]{1,64}\\x0a(CNF|DEL):([0-9a-f]){8}-(([0-9a-f]){4}-){3}([0-9a-f]){12}$";

		public static Regex ReservedADNameStringRegex = new Regex(ADObjectNameHelper.reservedStringPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
