using System;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	[Serializable]
	internal struct SmtpToken
	{
		static SmtpToken()
		{
			for (int i = 0; i < "[<(".Length; i++)
			{
				SmtpToken.charMapper[(int)"[<("[i]].ClosingDelimiter = "]>)"[i];
				SmtpToken.charMapper[(int)"[<("[i]].OpenDelimiter = true;
				SmtpToken.charMapper[(int)"]>)"[i]].CloseDelimiter = true;
			}
		}

		public static string SplitString(string input, char searchChar, out string tail)
		{
			int num = 0;
			bool flag = false;
			char[] array = new char[64];
			tail = null;
			int num2 = input.Length - 1;
			for (int i = 0; i <= num2; i++)
			{
				char c = input[i];
				if (c < '\u0080')
				{
					if (!flag && num == 0 && c == searchChar)
					{
						tail = input.Substring(i + 1);
						return input.Substring(0, i);
					}
					if (c == '\\')
					{
						i++;
						if (i > num2)
						{
							throw new FormatException(Strings.TrailingEscape);
						}
					}
					else
					{
						bool flag2 = true;
						if (flag)
						{
							flag2 = false;
							if (c == '"')
							{
								flag = false;
							}
						}
						else if (c == '"')
						{
							flag = true;
							flag2 = false;
						}
						else if (SmtpToken.charMapper[(int)c].OpenDelimiter)
						{
							flag2 = false;
							if (num == 64)
							{
								throw new FormatException(Strings.QuoteNestLevel);
							}
							array[num++] = SmtpToken.charMapper[(int)c].ClosingDelimiter;
						}
						if (flag2 && SmtpToken.charMapper[(int)c].CloseDelimiter)
						{
							if (num <= 0)
							{
								return input;
							}
							if (array[num - 1] != c)
							{
								throw new FormatException(Strings.IncorrectBrace);
							}
							num--;
							if (num == 0 && c == searchChar)
							{
								tail = input.Substring(i + 1);
								return input.Substring(0, i);
							}
						}
					}
				}
			}
			return input;
		}

		internal static bool IsA(char ch)
		{
			return ch >= 'A' && ch <= 'z' && (ch <= 'Z' || ch >= 'a');
		}

		internal static bool IsC(char ch)
		{
			return ch <= '\u007f' && ch != ' ' && !SmtpToken.IsSpecialOrControl(ch);
		}

		internal static bool IsD(char ch)
		{
			return ch >= '0' && ch <= '9';
		}

		internal static bool IsQ(char ch)
		{
			return ch <= '\u007f' && ch != '"' && ch != '\\' && !SmtpToken.IsCrOrLf(ch);
		}

		private static bool IsCrOrLf(char ch)
		{
			return ch == '\r' || ch == '\n';
		}

		private static bool IsControl(char ch)
		{
			return (ch >= '\0' && ch <= '\u001f') || ch == '\u007f';
		}

		private static bool IsSpecialOrControl(char ch)
		{
			if (ch <= '.')
			{
				if (ch != '"')
				{
					switch (ch)
					{
					case '(':
					case ')':
					case ',':
					case '.':
						break;
					case '*':
					case '+':
					case '-':
						goto IL_71;
					default:
						goto IL_71;
					}
				}
			}
			else
			{
				switch (ch)
				{
				case ':':
				case ';':
				case '<':
				case '>':
				case '@':
					break;
				case '=':
				case '?':
					goto IL_71;
				default:
					switch (ch)
					{
					case '[':
					case '\\':
					case ']':
						break;
					default:
						goto IL_71;
					}
					break;
				}
			}
			return true;
			IL_71:
			return SmtpToken.IsControl(ch);
		}

		private static SmtpToken.CharMapper[] charMapper = new SmtpToken.CharMapper[128];

		internal struct CharMapper
		{
			public char ClosingDelimiter;

			public bool OpenDelimiter;

			public bool CloseDelimiter;
		}
	}
}
