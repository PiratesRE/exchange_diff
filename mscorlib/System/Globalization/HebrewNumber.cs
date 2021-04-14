using System;
using System.Text;

namespace System.Globalization
{
	internal class HebrewNumber
	{
		private HebrewNumber()
		{
		}

		internal static string ToString(int Number)
		{
			char c = '\0';
			StringBuilder stringBuilder = new StringBuilder();
			if (Number > 5000)
			{
				Number -= 5000;
			}
			int num = Number / 100;
			if (num > 0)
			{
				Number -= num * 100;
				for (int i = 0; i < num / 4; i++)
				{
					stringBuilder.Append('ת');
				}
				int num2 = num % 4;
				if (num2 > 0)
				{
					stringBuilder.Append((char)(1510 + num2));
				}
			}
			int num3 = Number / 10;
			Number %= 10;
			switch (num3)
			{
			case 0:
				c = '\0';
				break;
			case 1:
				c = 'י';
				break;
			case 2:
				c = 'כ';
				break;
			case 3:
				c = 'ל';
				break;
			case 4:
				c = 'מ';
				break;
			case 5:
				c = 'נ';
				break;
			case 6:
				c = 'ס';
				break;
			case 7:
				c = 'ע';
				break;
			case 8:
				c = 'פ';
				break;
			case 9:
				c = 'צ';
				break;
			}
			char c2 = (char)((Number > 0) ? (1488 + Number - 1) : 0);
			if (c2 == 'ה' && c == 'י')
			{
				c2 = 'ו';
				c = 'ט';
			}
			if (c2 == 'ו' && c == 'י')
			{
				c2 = 'ז';
				c = 'ט';
			}
			if (c != '\0')
			{
				stringBuilder.Append(c);
			}
			if (c2 != '\0')
			{
				stringBuilder.Append(c2);
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Insert(stringBuilder.Length - 1, '"');
			}
			else
			{
				stringBuilder.Append('\'');
			}
			return stringBuilder.ToString();
		}

		internal static HebrewNumberParsingState ParseByChar(char ch, ref HebrewNumberParsingContext context)
		{
			HebrewNumber.HebrewToken hebrewToken;
			if (ch == '\'')
			{
				hebrewToken = HebrewNumber.HebrewToken.SingleQuote;
			}
			else if (ch == '"')
			{
				hebrewToken = HebrewNumber.HebrewToken.DoubleQuote;
			}
			else
			{
				int num = (int)(ch - 'א');
				if (num < 0 || num >= HebrewNumber.HebrewValues.Length)
				{
					return HebrewNumberParsingState.NotHebrewDigit;
				}
				hebrewToken = HebrewNumber.HebrewValues[num].token;
				if (hebrewToken == HebrewNumber.HebrewToken.Invalid)
				{
					return HebrewNumberParsingState.NotHebrewDigit;
				}
				context.result += HebrewNumber.HebrewValues[num].value;
			}
			context.state = HebrewNumber.NumberPasingState[(int)context.state][(int)hebrewToken];
			if (context.state == HebrewNumber.HS._err)
			{
				return HebrewNumberParsingState.InvalidHebrewNumber;
			}
			if (context.state == HebrewNumber.HS.END)
			{
				return HebrewNumberParsingState.FoundEndOfHebrewNumber;
			}
			return HebrewNumberParsingState.ContinueParsing;
		}

		internal static bool IsDigit(char ch)
		{
			if (ch >= 'א' && ch <= HebrewNumber.maxHebrewNumberCh)
			{
				return HebrewNumber.HebrewValues[(int)(ch - 'א')].value >= 0;
			}
			return ch == '\'' || ch == '"';
		}

		private static HebrewNumber.HebrewValue[] HebrewValues = new HebrewNumber.HebrewValue[]
		{
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit1, 1),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit1, 2),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit1, 3),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit1, 4),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit1, 5),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit6_7, 6),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit6_7, 7),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit1, 8),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit9, 9),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit10, 10),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Invalid, -1),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit10, 20),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit10, 30),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Invalid, -1),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit10, 40),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Invalid, -1),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit10, 50),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit10, 60),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit10, 70),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Invalid, -1),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit10, 80),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Invalid, -1),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit10, 90),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit100, 100),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit200_300, 200),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit200_300, 300),
			new HebrewNumber.HebrewValue(HebrewNumber.HebrewToken.Digit400, 400)
		};

		private const int minHebrewNumberCh = 1488;

		private static char maxHebrewNumberCh = (char)(1488 + HebrewNumber.HebrewValues.Length - 1);

		private static readonly HebrewNumber.HS[][] NumberPasingState = new HebrewNumber.HS[][]
		{
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS.S400,
				HebrewNumber.HS.X00,
				HebrewNumber.HS.X00,
				HebrewNumber.HS.X0,
				HebrewNumber.HS.X,
				HebrewNumber.HS.X,
				HebrewNumber.HS.X,
				HebrewNumber.HS.S9,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS.S400_400,
				HebrewNumber.HS.S400_X00,
				HebrewNumber.HS.S400_X00,
				HebrewNumber.HS.S400_X0,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.X00_S9,
				HebrewNumber.HS.END,
				HebrewNumber.HS.S400_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.S400_400_100,
				HebrewNumber.HS.S400_X0,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.X00_S9,
				HebrewNumber.HS._err,
				HebrewNumber.HS.S400_400_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.S400_X00_X0,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.X00_S9,
				HebrewNumber.HS._err,
				HebrewNumber.HS.X00_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.X0_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.X0_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.END,
				HebrewNumber.HS._err
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.END,
				HebrewNumber.HS.X0_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.S400_X0,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.X00_S9,
				HebrewNumber.HS.END,
				HebrewNumber.HS.X00_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.S400_X00_X0,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.X00_S9,
				HebrewNumber.HS._err,
				HebrewNumber.HS.X00_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.END,
				HebrewNumber.HS.S9_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.S9_DQ
			},
			new HebrewNumber.HS[]
			{
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS.END,
				HebrewNumber.HS.END,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err,
				HebrewNumber.HS._err
			}
		};

		private enum HebrewToken
		{
			Invalid = -1,
			Digit400,
			Digit200_300,
			Digit100,
			Digit10,
			Digit1,
			Digit6_7,
			Digit7,
			Digit9,
			SingleQuote,
			DoubleQuote
		}

		private class HebrewValue
		{
			internal HebrewValue(HebrewNumber.HebrewToken token, int value)
			{
				this.token = token;
				this.value = value;
			}

			internal HebrewNumber.HebrewToken token;

			internal int value;
		}

		internal enum HS
		{
			_err = -1,
			Start,
			S400,
			S400_400,
			S400_X00,
			S400_X0,
			X00_DQ,
			S400_X00_X0,
			X0_DQ,
			X,
			X0,
			X00,
			S400_DQ,
			S400_400_DQ,
			S400_400_100,
			S9,
			X00_S9,
			S9_DQ,
			END = 100
		}
	}
}
