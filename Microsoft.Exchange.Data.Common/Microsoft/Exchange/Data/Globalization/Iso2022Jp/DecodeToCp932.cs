using System;

namespace Microsoft.Exchange.Data.Globalization.Iso2022Jp
{
	internal abstract class DecodeToCp932
	{
		public abstract bool IsEscapeSequenceHandled(Escape escape);

		public abstract ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int bytesConsumed, out int bytesProduced);

		public abstract void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete);

		public static void MapEscape(byte[] dataIn, int offsetIn, int lengthIn, Escape escape)
		{
			int num = 0;
			EscapeState state = escape.State;
			int num2 = offsetIn;
			while (lengthIn > 0)
			{
				byte b = dataIn[offsetIn];
				EscapeSequence sequence;
				switch (escape.State)
				{
				case EscapeState.Begin:
					if (b == 27)
					{
						escape.State = EscapeState.Esc_1;
					}
					else
					{
						if (b == 14)
						{
							sequence = EscapeSequence.ShiftOut;
							goto IL_33B;
						}
						if (b == 15)
						{
							sequence = EscapeSequence.ShiftIn;
							goto IL_33B;
						}
						escape.BytesInCurrentBuffer = 0;
						escape.TotalBytes = 0;
						escape.Sequence = EscapeSequence.None;
						return;
					}
					break;
				case EscapeState.Esc_1:
					if (b == 36)
					{
						escape.State = EscapeState.Esc_Dollar_2;
					}
					else if (b == 40)
					{
						escape.State = EscapeState.Esc_OpenParen_2;
					}
					else
					{
						if (b == 72)
						{
							sequence = EscapeSequence.NECKanjiIn;
							goto IL_33B;
						}
						if (b == 75)
						{
							sequence = EscapeSequence.JisX0208_Nec;
							goto IL_33B;
						}
						if (b == 38)
						{
							escape.State = EscapeState.Esc_Ampersand_2;
						}
						else
						{
							if (b == 27)
							{
								goto IL_2C6;
							}
							if (b == 14 || b == 15)
							{
								goto IL_2CF;
							}
							if (b == 0)
							{
								goto IL_366;
							}
							goto IL_366;
						}
					}
					break;
				case EscapeState.Esc_Dollar_2:
					if (b == 64)
					{
						sequence = EscapeSequence.JisX0208_1983;
						goto IL_33B;
					}
					if (b == 65)
					{
						sequence = EscapeSequence.Gb2312_1980;
						goto IL_33B;
					}
					if (b == 66)
					{
						sequence = EscapeSequence.JisX0208_1978;
						goto IL_33B;
					}
					if (b == 40)
					{
						escape.State = EscapeState.Esc_Dollar_OpenParen_3;
					}
					else if (b == 41)
					{
						escape.State = EscapeState.Esc_Dollar_CloseParen_3;
					}
					else
					{
						if (b == 27)
						{
							goto IL_2C6;
						}
						if (b == 14)
						{
							goto IL_2CF;
						}
						if (b == 15)
						{
							goto IL_2CF;
						}
						goto IL_366;
					}
					break;
				case EscapeState.Esc_OpenParen_2:
					if (b == 73)
					{
						sequence = EscapeSequence.JisX0201K_1976;
						goto IL_33B;
					}
					if (b == 74)
					{
						sequence = EscapeSequence.JisX0201_1976;
						goto IL_33B;
					}
					if (b == 68)
					{
						sequence = EscapeSequence.JisX0212_1990;
						goto IL_33B;
					}
					if (b == 66)
					{
						sequence = EscapeSequence.Iso646Irv;
						goto IL_33B;
					}
					if (b == 27)
					{
						goto IL_2C6;
					}
					if (b == 14)
					{
						goto IL_2CF;
					}
					if (b == 15)
					{
						goto IL_2CF;
					}
					goto IL_366;
				case EscapeState.Esc_Ampersand_2:
					if (b == 64)
					{
						escape.State = EscapeState.Esc_Ampersand_At_3;
					}
					else
					{
						if (b == 27)
						{
							goto IL_2C6;
						}
						if (b == 14)
						{
							goto IL_2CF;
						}
						if (b == 15)
						{
							goto IL_2CF;
						}
						goto IL_366;
					}
					break;
				case EscapeState.Esc_K_2:
					goto IL_2F9;
				case EscapeState.Esc_Dollar_OpenParen_3:
					if (b == 71)
					{
						sequence = EscapeSequence.Cns11643_1992_1;
						goto IL_33B;
					}
					if (b == 67)
					{
						sequence = EscapeSequence.Kcs5601_1987;
						goto IL_33B;
					}
					if (b == 72)
					{
						sequence = EscapeSequence.Cns11643_1992_1;
						goto IL_33B;
					}
					if (b == 81)
					{
						sequence = EscapeSequence.Unknown_1;
						goto IL_33B;
					}
					if (b == 27)
					{
						goto IL_2C6;
					}
					if (b == 14)
					{
						goto IL_2CF;
					}
					if (b == 15)
					{
						goto IL_2CF;
					}
					goto IL_366;
				case EscapeState.Esc_Dollar_CloseParen_3:
					if (b == 67)
					{
						sequence = EscapeSequence.EucKsc;
						goto IL_33B;
					}
					if (b == 27)
					{
						goto IL_2C6;
					}
					if (b == 14)
					{
						goto IL_2CF;
					}
					if (b == 15)
					{
						goto IL_2CF;
					}
					goto IL_366;
				case EscapeState.Esc_Ampersand_At_3:
					if (b == 27)
					{
						escape.State = EscapeState.Esc_Ampersand_At_Esc_4;
					}
					else
					{
						if (b == 14)
						{
							goto IL_2CF;
						}
						if (b == 15)
						{
							goto IL_2CF;
						}
						goto IL_366;
					}
					break;
				case EscapeState.Esc_Ampersand_At_Esc_4:
					if (b == 36)
					{
						escape.State = EscapeState.Esc_Ampersand_At_Esc_Dollar_5;
					}
					else
					{
						if (b == 27)
						{
							goto IL_2C6;
						}
						if (b == 14)
						{
							goto IL_2CF;
						}
						if (b == 15)
						{
							goto IL_2CF;
						}
						goto IL_366;
					}
					break;
				case EscapeState.Esc_Ampersand_At_Esc_Dollar_5:
					if (b == 66)
					{
						sequence = EscapeSequence.JisX0208_1990;
						goto IL_33B;
					}
					if (b == 27)
					{
						goto IL_2C6;
					}
					if (b == 14)
					{
						goto IL_2CF;
					}
					if (b == 15)
					{
						goto IL_2CF;
					}
					goto IL_366;
				case EscapeState.Esc_Esc_Reset:
					goto IL_2C6;
				case EscapeState.Esc_SISO_Reset:
					goto IL_2CF;
				default:
					goto IL_2F9;
				}
				IL_304:
				lengthIn--;
				offsetIn++;
				num++;
				continue;
				IL_2C6:
				escape.State = EscapeState.Esc_1;
				goto IL_304;
				IL_2CF:
				if (b == 14)
				{
					sequence = EscapeSequence.ShiftOut;
					goto IL_33B;
				}
				if (b == 15)
				{
					sequence = EscapeSequence.ShiftIn;
					goto IL_33B;
				}
				throw new InvalidOperationException(string.Format("MapEscape: at Esc_SISO_Reset with {0}", (int)b));
				IL_2F9:
				throw new InvalidOperationException("MapEscape: unrecognized state!");
				IL_33B:
				escape.BytesInCurrentBuffer = num + 1;
				escape.TotalBytes += escape.BytesInCurrentBuffer;
				escape.State = EscapeState.Begin;
				escape.Sequence = sequence;
				return;
				IL_366:
				string text = string.Empty;
				while (num2 <= offsetIn && num2 < offsetIn + lengthIn)
				{
					text += dataIn[num2++].ToString("X2");
				}
				string.Format("Unrecognized escape sequence {0}, initial state {1}, current state {2}", text, state.ToString(), escape.State.ToString());
				escape.State = EscapeState.Begin;
				escape.Sequence = EscapeSequence.NotRecognized;
				escape.BytesInCurrentBuffer = num;
				escape.TotalBytes += num;
				return;
			}
			escape.BytesInCurrentBuffer = num;
			escape.TotalBytes += escape.BytesInCurrentBuffer;
			escape.Sequence = EscapeSequence.Incomplete;
		}

		public static DecodeToCp932[] GetStandardOrder()
		{
			DecodeToCp932[] result = new DecodeToCp932[]
			{
				new DecodeUsAsciiToCp932(),
				new DecodeJisX0208_1983ToCp932(),
				new DecodeJisX0201_1976ToCp932(),
				new DecodeLastChanceToCp932()
			};
			DecodeToCp932.LastChanceIndex = 3;
			return result;
		}

		public static int LastChanceDecoderIndex
		{
			get
			{
				return DecodeToCp932.LastChanceIndex;
			}
		}

		public virtual void CheckLoopCount(ref int count, int limit)
		{
			if (count > limit)
			{
				string name = base.GetType().Name;
				string message = string.Format("{0} decoder failed to progress", name);
				throw new FailedToProgressException(message);
			}
			count++;
		}

		public virtual int CalculateLoopCountLimit(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return length * 6 + 1000;
		}

		public virtual void Reset()
		{
		}

		public virtual char Abbreviation
		{
			get
			{
				return 'X';
			}
		}

		private static int LastChanceIndex;
	}
}
