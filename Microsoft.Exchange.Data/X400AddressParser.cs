using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal static class X400AddressParser
	{
		public static bool TryParse(string s, out IList<string> components)
		{
			bool flag;
			return X400AddressParser.TryParse(s, 21, false, false, out components, out flag);
		}

		public static bool TryParse(string s, int maxComponentsCount, bool addressSpace, bool locallyScoped, out IList<string> components)
		{
			bool flag;
			return X400AddressParser.TryParse(s, maxComponentsCount, addressSpace, locallyScoped, out components, out flag);
		}

		public static bool TryGetCanonical(IList<string> components, bool stripDdas, out string canonicalAddress)
		{
			if (components.Count > X400AddressParser.CanonicalOrder.Length)
			{
				canonicalAddress = null;
				return false;
			}
			int num = X400AddressParser.CanonicalOrder.Length;
			if (stripDdas)
			{
				num -= 4;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < num; i++)
			{
				X400ComponentType x400ComponentType = X400AddressParser.CanonicalOrder[i];
				if (x400ComponentType < (X400ComponentType)components.Count)
				{
					string text = components[(int)x400ComponentType];
					if (text != null)
					{
						X400AddressParser.AddComponent(stringBuilder, x400ComponentType, text);
					}
				}
			}
			canonicalAddress = stringBuilder.ToString();
			return true;
		}

		public static bool TryGetCanonical(string inputAddress, bool stripDdas, out string canonicalAddress, out bool endingWithSemicolon)
		{
			canonicalAddress = null;
			IList<string> components = null;
			return X400AddressParser.TryParse(inputAddress, 21, false, false, out components, out endingWithSemicolon) && X400AddressParser.TryGetCanonical(components, stripDdas, out canonicalAddress);
		}

		public static string ToCanonicalString(IList<string> components)
		{
			string result = null;
			if (X400AddressParser.TryGetCanonical(components, false, out result))
			{
				return result;
			}
			throw new ArgumentOutOfRangeException();
		}

		public static bool GetCanonical(string inputAddress, bool stripDdas, out string canonicalAddress)
		{
			bool result = false;
			if (X400AddressParser.TryGetCanonical(inputAddress, stripDdas, out canonicalAddress, out result))
			{
				return result;
			}
			throw new ArgumentOutOfRangeException(DataStrings.InvalidX400AddressSpace(inputAddress));
		}

		private static bool TryParse(string s, int maxComponentsCount, bool addressSpace, bool locallyScoped, out IList<string> components, out bool endingWithSemicolon)
		{
			if (maxComponentsCount <= 0 || maxComponentsCount > 21)
			{
				throw new InvalidOperationException("maxComponentsCount is out of range");
			}
			endingWithSemicolon = false;
			components = null;
			if (s == null)
			{
				return false;
			}
			int num = 0;
			char c = '/';
			if (s.Length >= 2 && s[0] == c)
			{
				num++;
			}
			else
			{
				c = ';';
			}
			List<string> list = new List<string>(maxComponentsCount);
			for (int i = 0; i < maxComponentsCount; i++)
			{
				list.Add(null);
			}
			int num2 = 0;
			int j = num;
			int length = s.Length;
			while (j < length)
			{
				while (j < length && (' ' == s[j] || '\t' == s[j]))
				{
					j++;
				}
				if (j == length)
				{
					break;
				}
				int num3 = s.IndexOfAny(X400AddressParser.KeySeparators, j);
				if (-1 == num3 || j == num3)
				{
					return false;
				}
				X400ComponentType componentType = X400AddressParser.GetComponentType(s, j, num3 - j);
				if (X400ComponentType.Unsupported == componentType)
				{
					return false;
				}
				if ((componentType == X400ComponentType.DDA && s[num3] != X400AddressParser.KeySeparators[1]) || (componentType != X400ComponentType.DDA && s[num3] != X400AddressParser.KeySeparators[0]))
				{
					return false;
				}
				if (addressSpace && X400ComponentType.X121Address == componentType)
				{
					return false;
				}
				string text;
				j = X400AddressParser.GetUnescapedValue(s, num3 + 1, locallyScoped, c, out text, out endingWithSemicolon);
				if (X400AddressParser.MaxComponentLengths[(int)componentType] < text.Length)
				{
					return false;
				}
				if (componentType == X400ComponentType.DDA && num2++ == 4)
				{
					return false;
				}
				if (maxComponentsCount > (int)componentType)
				{
					if (componentType != X400ComponentType.DDA)
					{
						list[(int)componentType] = text;
					}
					else
					{
						int num4 = 17 + num2 - 1;
						if (num4 < maxComponentsCount)
						{
							list[num4] = text;
						}
					}
				}
			}
			bool flag = false;
			for (int k = 4; k <= 7; k++)
			{
				if (flag)
				{
					list[k] = null;
				}
				else
				{
					flag = string.IsNullOrEmpty(list[k]);
				}
			}
			components = list;
			return true;
		}

		private static X400ComponentType GetComponentType(string s, int startIndex, int length)
		{
			for (int i = 0; i < X400AddressParser.Keys.Length; i++)
			{
				if (length == X400AddressParser.Keys[i].Length && string.Compare(X400AddressParser.Keys[i], 0, s, startIndex, length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return (X400ComponentType)i;
				}
			}
			return X400ComponentType.Unsupported;
		}

		private static int GetUnescapedValue(string s, int startIndex, bool locallyScoped, char delimiter, out string value, out bool brokenBySeparator)
		{
			brokenBySeparator = false;
			StringBuilder stringBuilder = null;
			bool flag = false;
			int i = startIndex;
			int length = s.Length;
			while (i < length)
			{
				if (flag)
				{
					goto IL_84;
				}
				if (locallyScoped && s[i] == ',')
				{
					brokenBySeparator = true;
					break;
				}
				if (s[i] == delimiter && (length - 1 == i || s[i + 1] != delimiter))
				{
					brokenBySeparator = true;
					break;
				}
				if ('\\' != s[i] && s[i] != delimiter)
				{
					goto IL_84;
				}
				if (stringBuilder == null)
				{
					if (i > startIndex)
					{
						stringBuilder = new StringBuilder(s, startIndex, i - startIndex, length - startIndex);
					}
					else
					{
						startIndex++;
					}
				}
				flag = true;
				IL_97:
				i++;
				continue;
				IL_84:
				if (stringBuilder != null)
				{
					stringBuilder.Append(s[i]);
				}
				flag = false;
				goto IL_97;
			}
			if (stringBuilder == null)
			{
				if (startIndex >= length || i == startIndex)
				{
					value = string.Empty;
				}
				else
				{
					value = s.Substring(startIndex, i - startIndex);
				}
			}
			else
			{
				value = stringBuilder.ToString();
			}
			if (i >= length)
			{
				return i;
			}
			return i + 1;
		}

		private static void AddComponent(StringBuilder builder, X400ComponentType type, string value)
		{
			if (type < X400ComponentType.DDA)
			{
				builder.Append(X400AddressParser.Keys[(int)type]);
				builder.Append(X400AddressParser.KeySeparators[0]);
			}
			else
			{
				builder.Append(X400AddressParser.Keys[17]);
				builder.Append(X400AddressParser.KeySeparators[1]);
			}
			foreach (char c in value)
			{
				if (c == ';' || c == '\\')
				{
					builder.Append('\\');
				}
				builder.Append(c);
			}
			builder.Append(';');
		}

		private const int MaxComponentsCount = 21;

		private const char ComponentSeparator = ';';

		private const char AlternateComponentSeparator = '/';

		private const char LocalComponentSeparator = ',';

		private const char EscChar = '\\';

		private static readonly char[] KeySeparators = new char[]
		{
			'=',
			':'
		};

		private static readonly string[] Keys = new string[]
		{
			"C",
			"A",
			"P",
			"O",
			"OU1",
			"OU2",
			"OU3",
			"OU4",
			"CN",
			"S",
			"G",
			"I",
			"Q",
			"N-ID",
			"X.121",
			"T-ID",
			"T-TY",
			"DDA"
		};

		public static readonly int[] MaxComponentLengths = new int[]
		{
			3,
			16,
			16,
			64,
			32,
			32,
			32,
			32,
			64,
			40,
			16,
			5,
			3,
			32,
			15,
			24,
			3,
			137
		};

		private static readonly X400ComponentType[] CanonicalOrder = new X400ComponentType[]
		{
			X400ComponentType.Country,
			X400ComponentType.ADMD,
			X400ComponentType.PRMD,
			X400ComponentType.Organization,
			X400ComponentType.Surname,
			X400ComponentType.GivenName,
			X400ComponentType.Initials,
			X400ComponentType.Generation,
			X400ComponentType.CommonName,
			X400ComponentType.OrgUnit1,
			X400ComponentType.OrgUnit2,
			X400ComponentType.OrgUnit3,
			X400ComponentType.OrgUnit4,
			X400ComponentType.X121Address,
			X400ComponentType.NumericUserId,
			X400ComponentType.TerminalType,
			X400ComponentType.TerminalId,
			X400ComponentType.DDA,
			X400ComponentType.DDA2,
			X400ComponentType.DDA3,
			X400ComponentType.DDA4
		};
	}
}
