using System;
using System.Globalization;
using System.Security;
using System.Threading;

namespace System
{
	internal struct __DTString
	{
		internal __DTString(string str, DateTimeFormatInfo dtfi, bool checkDigitToken)
		{
			this = new __DTString(str, dtfi);
			this.m_checkDigitToken = checkDigitToken;
		}

		internal __DTString(string str, DateTimeFormatInfo dtfi)
		{
			this.Index = -1;
			this.Value = str;
			this.len = this.Value.Length;
			this.m_current = '\0';
			if (dtfi != null)
			{
				this.m_info = dtfi.CompareInfo;
				this.m_checkDigitToken = ((dtfi.FormatFlags & DateTimeFormatFlags.UseDigitPrefixInTokens) > DateTimeFormatFlags.None);
				return;
			}
			this.m_info = Thread.CurrentThread.CurrentCulture.CompareInfo;
			this.m_checkDigitToken = false;
		}

		internal CompareInfo CompareInfo
		{
			get
			{
				return this.m_info;
			}
		}

		internal bool GetNext()
		{
			this.Index++;
			if (this.Index < this.len)
			{
				this.m_current = this.Value[this.Index];
				return true;
			}
			return false;
		}

		internal bool AtEnd()
		{
			return this.Index >= this.len;
		}

		internal bool Advance(int count)
		{
			this.Index += count;
			if (this.Index < this.len)
			{
				this.m_current = this.Value[this.Index];
				return true;
			}
			return false;
		}

		[SecurityCritical]
		internal void GetRegularToken(out TokenType tokenType, out int tokenValue, DateTimeFormatInfo dtfi)
		{
			tokenValue = 0;
			if (this.Index >= this.len)
			{
				tokenType = TokenType.EndOfString;
				return;
			}
			tokenType = TokenType.UnknownToken;
			IL_19:
			while (!DateTimeParse.IsDigit(this.m_current))
			{
				if (char.IsWhiteSpace(this.m_current))
				{
					for (;;)
					{
						int num = this.Index + 1;
						this.Index = num;
						if (num >= this.len)
						{
							break;
						}
						this.m_current = this.Value[this.Index];
						if (!char.IsWhiteSpace(this.m_current))
						{
							goto IL_19;
						}
					}
					tokenType = TokenType.EndOfString;
					return;
				}
				dtfi.Tokenize(TokenType.RegularTokenMask, out tokenType, out tokenValue, ref this);
				return;
			}
			tokenValue = (int)(this.m_current - '0');
			int index = this.Index;
			for (;;)
			{
				int num = this.Index + 1;
				this.Index = num;
				if (num >= this.len)
				{
					break;
				}
				this.m_current = this.Value[this.Index];
				int num2 = (int)(this.m_current - '0');
				if (num2 < 0 || num2 > 9)
				{
					break;
				}
				tokenValue = tokenValue * 10 + num2;
			}
			if (this.Index - index > 8)
			{
				tokenType = TokenType.NumberToken;
				tokenValue = -1;
			}
			else if (this.Index - index < 3)
			{
				tokenType = TokenType.NumberToken;
			}
			else
			{
				tokenType = TokenType.YearNumberToken;
			}
			if (!this.m_checkDigitToken)
			{
				return;
			}
			int index2 = this.Index;
			char current = this.m_current;
			this.Index = index;
			this.m_current = this.Value[this.Index];
			TokenType tokenType2;
			int num3;
			if (dtfi.Tokenize(TokenType.RegularTokenMask, out tokenType2, out num3, ref this))
			{
				tokenType = tokenType2;
				tokenValue = num3;
				return;
			}
			this.Index = index2;
			this.m_current = current;
		}

		[SecurityCritical]
		internal TokenType GetSeparatorToken(DateTimeFormatInfo dtfi, out int indexBeforeSeparator, out char charBeforeSeparator)
		{
			indexBeforeSeparator = this.Index;
			charBeforeSeparator = this.m_current;
			if (!this.SkipWhiteSpaceCurrent())
			{
				return TokenType.SEP_End;
			}
			TokenType result;
			if (!DateTimeParse.IsDigit(this.m_current))
			{
				int num;
				if (!dtfi.Tokenize(TokenType.SeparatorTokenMask, out result, out num, ref this))
				{
					result = TokenType.SEP_Space;
				}
			}
			else
			{
				result = TokenType.SEP_Space;
			}
			return result;
		}

		internal bool MatchSpecifiedWord(string target)
		{
			return this.MatchSpecifiedWord(target, target.Length + this.Index);
		}

		internal bool MatchSpecifiedWord(string target, int endIndex)
		{
			int num = endIndex - this.Index;
			return num == target.Length && this.Index + num <= this.len && this.m_info.Compare(this.Value, this.Index, num, target, 0, num, CompareOptions.IgnoreCase) == 0;
		}

		internal bool MatchSpecifiedWords(string target, bool checkWordBoundary, ref int matchLength)
		{
			int num = this.Value.Length - this.Index;
			matchLength = target.Length;
			if (matchLength > num || this.m_info.Compare(this.Value, this.Index, matchLength, target, 0, matchLength, CompareOptions.IgnoreCase) != 0)
			{
				int num2 = 0;
				int num3 = this.Index;
				int num4 = target.IndexOfAny(__DTString.WhiteSpaceChecks, num2);
				if (num4 == -1)
				{
					return false;
				}
				for (;;)
				{
					int num5 = num4 - num2;
					if (num3 >= this.Value.Length - num5)
					{
						break;
					}
					if (num5 == 0)
					{
						matchLength--;
					}
					else
					{
						if (!char.IsWhiteSpace(this.Value[num3 + num5]))
						{
							return false;
						}
						if (this.m_info.Compare(this.Value, num3, num5, target, num2, num5, CompareOptions.IgnoreCase) != 0)
						{
							return false;
						}
						num3 = num3 + num5 + 1;
					}
					num2 = num4 + 1;
					while (num3 < this.Value.Length && char.IsWhiteSpace(this.Value[num3]))
					{
						num3++;
						matchLength++;
					}
					if ((num4 = target.IndexOfAny(__DTString.WhiteSpaceChecks, num2)) < 0)
					{
						goto Block_8;
					}
				}
				return false;
				Block_8:
				if (num2 < target.Length)
				{
					int num6 = target.Length - num2;
					if (num3 > this.Value.Length - num6)
					{
						return false;
					}
					if (this.m_info.Compare(this.Value, num3, num6, target, num2, num6, CompareOptions.IgnoreCase) != 0)
					{
						return false;
					}
				}
			}
			if (checkWordBoundary)
			{
				int num7 = this.Index + matchLength;
				if (num7 < this.Value.Length && char.IsLetter(this.Value[num7]))
				{
					return false;
				}
			}
			return true;
		}

		internal bool Match(string str)
		{
			int num = this.Index + 1;
			this.Index = num;
			if (num >= this.len)
			{
				return false;
			}
			if (str.Length > this.Value.Length - this.Index)
			{
				return false;
			}
			if (this.m_info.Compare(this.Value, this.Index, str.Length, str, 0, str.Length, CompareOptions.Ordinal) == 0)
			{
				this.Index += str.Length - 1;
				return true;
			}
			return false;
		}

		internal bool Match(char ch)
		{
			int num = this.Index + 1;
			this.Index = num;
			if (num >= this.len)
			{
				return false;
			}
			if (this.Value[this.Index] == ch)
			{
				this.m_current = ch;
				return true;
			}
			this.Index--;
			return false;
		}

		internal int MatchLongestWords(string[] words, ref int maxMatchStrLen)
		{
			int result = -1;
			for (int i = 0; i < words.Length; i++)
			{
				string text = words[i];
				int length = text.Length;
				if (this.MatchSpecifiedWords(text, false, ref length) && length > maxMatchStrLen)
				{
					maxMatchStrLen = length;
					result = i;
				}
			}
			return result;
		}

		internal int GetRepeatCount()
		{
			char c = this.Value[this.Index];
			int num = this.Index + 1;
			while (num < this.len && this.Value[num] == c)
			{
				num++;
			}
			int result = num - this.Index;
			this.Index = num - 1;
			return result;
		}

		internal bool GetNextDigit()
		{
			int num = this.Index + 1;
			this.Index = num;
			return num < this.len && DateTimeParse.IsDigit(this.Value[this.Index]);
		}

		internal char GetChar()
		{
			return this.Value[this.Index];
		}

		internal int GetDigit()
		{
			return (int)(this.Value[this.Index] - '0');
		}

		internal void SkipWhiteSpaces()
		{
			while (this.Index + 1 < this.len)
			{
				char c = this.Value[this.Index + 1];
				if (!char.IsWhiteSpace(c))
				{
					return;
				}
				this.Index++;
			}
		}

		internal bool SkipWhiteSpaceCurrent()
		{
			if (this.Index >= this.len)
			{
				return false;
			}
			if (!char.IsWhiteSpace(this.m_current))
			{
				return true;
			}
			do
			{
				int num = this.Index + 1;
				this.Index = num;
				if (num >= this.len)
				{
					return false;
				}
				this.m_current = this.Value[this.Index];
			}
			while (char.IsWhiteSpace(this.m_current));
			return true;
		}

		internal void TrimTail()
		{
			int num = this.len - 1;
			while (num >= 0 && char.IsWhiteSpace(this.Value[num]))
			{
				num--;
			}
			this.Value = this.Value.Substring(0, num + 1);
			this.len = this.Value.Length;
		}

		internal void RemoveTrailingInQuoteSpaces()
		{
			int num = this.len - 1;
			if (num <= 1)
			{
				return;
			}
			char c = this.Value[num];
			if ((c == '\'' || c == '"') && char.IsWhiteSpace(this.Value[num - 1]))
			{
				num--;
				while (num >= 1 && char.IsWhiteSpace(this.Value[num - 1]))
				{
					num--;
				}
				this.Value = this.Value.Remove(num, this.Value.Length - 1 - num);
				this.len = this.Value.Length;
			}
		}

		internal void RemoveLeadingInQuoteSpaces()
		{
			if (this.len <= 2)
			{
				return;
			}
			int num = 0;
			char c = this.Value[num];
			if (c != '\'')
			{
				if (c != '"')
				{
					return;
				}
			}
			while (num + 1 < this.len && char.IsWhiteSpace(this.Value[num + 1]))
			{
				num++;
			}
			if (num != 0)
			{
				this.Value = this.Value.Remove(1, num);
				this.len = this.Value.Length;
			}
		}

		internal DTSubString GetSubString()
		{
			DTSubString dtsubString = default(DTSubString);
			dtsubString.index = this.Index;
			dtsubString.s = this.Value;
			while (this.Index + dtsubString.length < this.len)
			{
				char c = this.Value[this.Index + dtsubString.length];
				DTSubStringType dtsubStringType;
				if (c >= '0' && c <= '9')
				{
					dtsubStringType = DTSubStringType.Number;
				}
				else
				{
					dtsubStringType = DTSubStringType.Other;
				}
				if (dtsubString.length == 0)
				{
					dtsubString.type = dtsubStringType;
				}
				else if (dtsubString.type != dtsubStringType)
				{
					break;
				}
				dtsubString.length++;
				if (dtsubStringType != DTSubStringType.Number)
				{
					break;
				}
				if (dtsubString.length > 8)
				{
					dtsubString.type = DTSubStringType.Invalid;
					return dtsubString;
				}
				int num = (int)(c - '0');
				dtsubString.value = dtsubString.value * 10 + num;
			}
			if (dtsubString.length == 0)
			{
				dtsubString.type = DTSubStringType.End;
				return dtsubString;
			}
			return dtsubString;
		}

		internal void ConsumeSubString(DTSubString sub)
		{
			this.Index = sub.index + sub.length;
			if (this.Index < this.len)
			{
				this.m_current = this.Value[this.Index];
			}
		}

		internal string Value;

		internal int Index;

		internal int len;

		internal char m_current;

		private CompareInfo m_info;

		private bool m_checkDigitToken;

		private static char[] WhiteSpaceChecks = new char[]
		{
			' ',
			'\u00a0'
		};
	}
}
