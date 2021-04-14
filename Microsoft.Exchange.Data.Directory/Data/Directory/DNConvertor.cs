using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class DNConvertor
	{
		public static string FqdnFromDomainDistinguishedName(string distinguishedName)
		{
			if (distinguishedName == null)
			{
				throw new ArgumentNullException("distinguishedName");
			}
			return distinguishedName.Substring(3).ToLowerInvariant().Replace(",dc=", ".");
		}

		public static string[] SplitDistinguishedName(string distinguishedName, char separator)
		{
			if (string.IsNullOrEmpty(distinguishedName))
			{
				throw new ArgumentNullException("distinguishedName");
			}
			List<string> list = new List<string>(distinguishedName.Length / 4);
			int num = 0;
			do
			{
				int num2 = DNConvertor.IndexOfUnescapedChar(distinguishedName, num, separator);
				if (num2 == -1)
				{
					num2 = distinguishedName.Length;
				}
				string text = distinguishedName.Substring(num, num2 - num);
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
				num = num2 + 1;
			}
			while (num < distinguishedName.Length);
			return list.ToArray();
		}

		internal static string ServerNameFromServerLegacyDN(string serverLegacyDN)
		{
			int num = serverLegacyDN.LastIndexOf("cn=", StringComparison.OrdinalIgnoreCase);
			if (num < 0 || num + 3 >= serverLegacyDN.Length)
			{
				return string.Empty;
			}
			int num2 = num + 3;
			StringBuilder stringBuilder = new StringBuilder(serverLegacyDN.Length - num2);
			for (int i = num2; i < serverLegacyDN.Length; i++)
			{
				stringBuilder.Append(char.ToLowerInvariant(serverLegacyDN[i]));
			}
			return stringBuilder.ToString();
		}

		internal static int LastIndexOfUnescapedChar(string input, int startIndex, int length, char ch)
		{
			int num = -1;
			bool flag = false;
			for (int i = startIndex; i > startIndex - length; i--)
			{
				if (input[i] == '\\')
				{
					flag = !flag;
				}
				else
				{
					if (num != -1)
					{
						if (!flag)
						{
							return num;
						}
						num = -1;
					}
					if (input[i] == ch)
					{
						num = i;
					}
					flag = false;
				}
			}
			if (num == -1 || flag)
			{
				return -1;
			}
			return num;
		}

		public static int LastIndexOfUnescapedChar(string input, int startIndex, char ch)
		{
			return DNConvertor.LastIndexOfUnescapedChar(input, startIndex, startIndex + 1, ch);
		}

		internal static int IndexOfUnescapedChar(string input, int startIndex, int length, char ch)
		{
			bool flag = false;
			for (int i = startIndex; i < startIndex + length; i++)
			{
				if (input[i] == '\\')
				{
					flag = !flag;
				}
				else
				{
					if (input[i] == ch && !flag)
					{
						return i;
					}
					flag = false;
				}
			}
			return -1;
		}

		public static int IndexOfUnescapedChar(string input, int startIndex, char ch)
		{
			return DNConvertor.IndexOfUnescapedChar(input, startIndex, input.Length - startIndex, ch);
		}
	}
}
