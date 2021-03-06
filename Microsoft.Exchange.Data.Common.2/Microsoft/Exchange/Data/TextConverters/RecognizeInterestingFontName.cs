using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal struct RecognizeInterestingFontName
	{
		public TextMapping TextMapping
		{
			get
			{
				switch (this.state)
				{
				case 1:
					return TextMapping.Symbol;
				case 2:
					return TextMapping.Wingdings;
				default:
					return TextMapping.Unicode;
				}
			}
		}

		public bool IsRejected
		{
			get
			{
				return this.state < 0;
			}
		}

		public void AddCharacter(byte ch)
		{
			if (this.state >= 0)
			{
				this.state = RecognizeInterestingFontName.stateTransitionTable[(int)this.state, (int)((ch > 127) ? 0 : RecognizeInterestingFontName.charMapToClass[(int)ch])];
			}
		}

		public void AddCharacter(char ch)
		{
			if (this.state >= 0)
			{
				this.state = RecognizeInterestingFontName.stateTransitionTable[(int)this.state, (int)((ch > '\u007f') ? 0 : RecognizeInterestingFontName.charMapToClass[(int)ch])];
			}
		}

		private static byte[] charMapToClass = new byte[]
		{
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			1,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			11,
			0,
			7,
			0,
			0,
			6,
			0,
			4,
			0,
			0,
			13,
			10,
			5,
			12,
			0,
			0,
			0,
			8,
			0,
			0,
			0,
			3,
			0,
			9,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			11,
			0,
			7,
			0,
			0,
			6,
			0,
			4,
			0,
			0,
			13,
			10,
			5,
			12,
			0,
			0,
			0,
			8,
			0,
			0,
			0,
			3,
			0,
			9,
			0,
			0,
			0,
			0,
			0,
			0
		};

		private static sbyte[,] stateTransitionTable = new sbyte[,]
		{
			{
				-1,
				0,
				-1,
				3,
				-1,
				-1,
				-1,
				-1,
				11,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				1,
				1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				2,
				2,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				4,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				5,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				6,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				7,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				8,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				9,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				10,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				2,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				12,
				-1,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				13,
				-1,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				14,
				-1,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				15,
				-1
			},
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				1
			}
		};

		private sbyte state;
	}
}
