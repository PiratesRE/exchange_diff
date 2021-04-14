using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal class WildcardPattern
	{
		public WildcardPattern(string pattern)
		{
			this.patternType = WildcardPattern.NormalizePattern(pattern, out this.pattern);
			if (WildcardPattern.PatternType.Mixed == this.patternType)
			{
				this.splitPattern = this.pattern.Split(WildcardPattern.WildcardChars, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		public string Pattern
		{
			get
			{
				return this.pattern;
			}
		}

		public WildcardPattern.PatternType Type
		{
			get
			{
				return this.patternType;
			}
		}

		private bool StartsWithWildcard
		{
			get
			{
				return '*' == this.pattern[0];
			}
		}

		private bool EndsWithWildcard
		{
			get
			{
				return '*' == this.pattern[this.pattern.Length - 1];
			}
		}

		public bool Equals(WildcardPattern other)
		{
			return this.pattern.Equals(other.pattern, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object other)
		{
			WildcardPattern wildcardPattern = other as WildcardPattern;
			return wildcardPattern != null && this.Equals(wildcardPattern);
		}

		public override int GetHashCode()
		{
			return this.pattern.GetHashCode();
		}

		public int Match(string address)
		{
			return this.Match(address, '\0');
		}

		public int Match(string address, char singleCharWildcard)
		{
			int num = 0;
			if (this.patternType == WildcardPattern.PatternType.NoWildcards)
			{
				num = WildcardPattern.WildcardEqual(address, this.pattern, singleCharWildcard);
				if (address.Length == num)
				{
					num++;
				}
				return num;
			}
			if (WildcardPattern.PatternType.Wildcard == this.patternType)
			{
				return 0;
			}
			int i = 0;
			int num2 = 0;
			int num3 = this.splitPattern.Length;
			if (!this.StartsWithWildcard)
			{
				num = WildcardPattern.WildcardStartsWith(address, this.splitPattern[0], singleCharWildcard);
				if (-1 == num)
				{
					return -1;
				}
				i++;
				num2 = this.splitPattern[0].Length;
			}
			int num4 = num3;
			if (!this.EndsWithWildcard)
			{
				num4--;
			}
			while (i < num4)
			{
				int num5;
				num2 = WildcardPattern.WildcardIndexOf(address, num2, this.splitPattern[i], singleCharWildcard, out num5);
				if (-1 == num2)
				{
					return -1;
				}
				num += num5;
				num2 += this.splitPattern[i].Length;
				i++;
			}
			if (num4 < num3)
			{
				int length = this.splitPattern[i].Length;
				if (num2 + length > address.Length)
				{
					return -1;
				}
				int num5 = WildcardPattern.WildcardEndsWith(address, this.splitPattern[i], singleCharWildcard);
				if (-1 == num5)
				{
					return -1;
				}
				num += num5;
			}
			return num;
		}

		public override string ToString()
		{
			return this.pattern;
		}

		private static WildcardPattern.PatternType NormalizePattern(string pattern, out string normalizedPattern)
		{
			bool flag = false;
			bool flag2 = false;
			StringBuilder stringBuilder = null;
			int i = 0;
			while (i < pattern.Length)
			{
				if ('*' != pattern[i])
				{
					flag2 = true;
					goto IL_40;
				}
				flag = true;
				if (i <= 0 || '*' != pattern[i - 1])
				{
					goto IL_40;
				}
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(pattern, 0, i, pattern.Length - 1);
				}
				IL_51:
				i++;
				continue;
				IL_40:
				if (stringBuilder != null)
				{
					stringBuilder.Append(pattern[i]);
					goto IL_51;
				}
				goto IL_51;
			}
			WildcardPattern.PatternType result;
			if (flag && flag2)
			{
				result = WildcardPattern.PatternType.Mixed;
				normalizedPattern = ((stringBuilder == null) ? pattern : stringBuilder.ToString());
			}
			else if (flag && !flag2)
			{
				result = WildcardPattern.PatternType.Wildcard;
				normalizedPattern = "*";
			}
			else
			{
				result = WildcardPattern.PatternType.NoWildcards;
				normalizedPattern = pattern;
			}
			return result;
		}

		private static int WildcardStartsWith(string str, string subStr, char singleCharWildcard)
		{
			if (singleCharWildcard == '\0')
			{
				if (!str.StartsWith(subStr, StringComparison.OrdinalIgnoreCase))
				{
					return -1;
				}
				return subStr.Length;
			}
			else
			{
				if (subStr.Length > str.Length)
				{
					return -1;
				}
				return WildcardPattern.WildcardEqual(str, 0, subStr, singleCharWildcard);
			}
		}

		private static int WildcardEndsWith(string str, string subStr, char singleCharWildcard)
		{
			if (singleCharWildcard == '\0')
			{
				if (!str.EndsWith(subStr, StringComparison.OrdinalIgnoreCase))
				{
					return -1;
				}
				return subStr.Length;
			}
			else
			{
				int length = str.Length;
				int length2 = subStr.Length;
				if (length2 > length)
				{
					return -1;
				}
				return WildcardPattern.WildcardEqual(str, length - length2, subStr, singleCharWildcard);
			}
		}

		private static int WildcardIndexOf(string str, int startIndex, string subStr, char singleCharWildcard, out int matchCount)
		{
			matchCount = -1;
			if (singleCharWildcard == '\0')
			{
				int num = str.IndexOf(subStr, startIndex, StringComparison.OrdinalIgnoreCase);
				if (num >= 0)
				{
					matchCount = subStr.Length;
				}
				return num;
			}
			int length = str.Length;
			int length2 = subStr.Length;
			for (int i = startIndex; i < length - length2 + 1; i++)
			{
				matchCount = WildcardPattern.WildcardEqual(str, i, subStr, singleCharWildcard);
				if (matchCount >= 0)
				{
					return i;
				}
			}
			return -1;
		}

		private static int WildcardEqual(string s1, string s2, char singleCharWildcard)
		{
			if (singleCharWildcard == '\0')
			{
				if (!s1.Equals(s2, StringComparison.OrdinalIgnoreCase))
				{
					return -1;
				}
				return s1.Length;
			}
			else
			{
				if (s1.Length != s2.Length)
				{
					return -1;
				}
				return WildcardPattern.WildcardEqual(s1, 0, s2, singleCharWildcard);
			}
		}

		private static int WildcardEqual(string str, int startIndex, string subStr, char singleCharWildcard)
		{
			int num = 0;
			int i = 0;
			int length = subStr.Length;
			while (i < length)
			{
				switch (WildcardPattern.WildcardEqualChars(str, startIndex + i, subStr, i, singleCharWildcard))
				{
				case -1:
					return -1;
				case 1:
					num++;
					break;
				}
				i++;
			}
			return num;
		}

		private static int WildcardEqualChars(string s1, int index1, string s2, int index2, char singleCharWildcard)
		{
			if (singleCharWildcard == s2[index2])
			{
				return 0;
			}
			if (string.Compare(s1, index1, s2, index2, 1, StringComparison.OrdinalIgnoreCase) != 0)
			{
				return -1;
			}
			return 1;
		}

		public const char WildcardChar = '*';

		public const string WildcardString = "*";

		private const char NoSingleCharWildcard = '\0';

		private static readonly char[] WildcardChars = new char[]
		{
			'*'
		};

		private string pattern;

		private WildcardPattern.PatternType patternType;

		private string[] splitPattern;

		internal enum PatternType
		{
			NoWildcards,
			Wildcard,
			Mixed
		}
	}
}
