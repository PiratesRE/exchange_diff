using System;

namespace Microsoft.Exchange.Data.Globalization.Iso2022Jp
{
	internal class DecodeJisX0201_1976ToCp932 : DecodeToCp932
	{
		public override bool IsEscapeSequenceHandled(Escape escape)
		{
			return escape.Sequence == EscapeSequence.JisX0201_1976 || escape.Sequence == EscapeSequence.JisX0201K_1976 || escape.Sequence == EscapeSequence.ShiftIn || escape.Sequence == EscapeSequence.ShiftOut;
		}

		public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut)
		{
			usedIn = 0;
			usedOut = 0;
			int i = offsetIn;
			int num = 0;
			int num2 = 0;
			int limit = this.CalculateLoopCountLimit(lengthIn);
			if (escape.IsValidEscapeSequence)
			{
				if (!this.IsEscapeSequenceHandled(escape))
				{
					throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
				}
				i += escape.BytesInCurrentBuffer;
				this.runBeganWithEscape = true;
			}
			else if (!this.runBeganWithEscape)
			{
				return ValidationResult.Invalid;
			}
			while (i < offsetIn + lengthIn)
			{
				this.CheckLoopCount(ref num2, limit);
				byte b = dataIn[i];
				if (b == 27 || b == 0)
				{
					break;
				}
				if (b != 14 && b != 15)
				{
					num++;
				}
				i++;
			}
			usedIn = i - offsetIn;
			usedOut = num;
			return ValidationResult.Valid;
		}

		public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete)
		{
			usedIn = 0;
			usedOut = 0;
			complete = false;
			int i = offsetIn;
			int num = offsetOut;
			int num2 = 0;
			int num3 = 0;
			int limit = this.CalculateLoopCountLimit(lengthIn);
			if (escape.IsValidEscapeSequence)
			{
				if (!this.IsEscapeSequenceHandled(escape))
				{
					throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
				}
				i += escape.BytesInCurrentBuffer;
				this.isKana = (escape.Sequence == EscapeSequence.JisX0201K_1976 || escape.Sequence == EscapeSequence.ShiftOut);
				this.isEscapeKana = (escape.Sequence == EscapeSequence.JisX0201K_1976);
			}
			while (i < offsetIn + lengthIn)
			{
				this.CheckLoopCount(ref num3, limit);
				byte b = dataIn[i];
				if (b == 27 || b == 0)
				{
					break;
				}
				if (b == 14)
				{
					this.isKana = true;
				}
				else if (b == 15)
				{
					this.isKana = this.isEscapeKana;
				}
				else if (b < 128)
				{
					if (!this.isKana || b < 33 || b > 95)
					{
						dataOut[num] = b;
					}
					else
					{
						dataOut[num] = b + 128;
					}
					num++;
					num2++;
				}
				else if (b >= 161 && b <= 223)
				{
					dataOut[num] = b;
					num++;
					num2++;
				}
				else if (b == 160)
				{
					dataOut[num] = 32;
					num++;
					num2++;
				}
				else
				{
					dataOut[num] = 63;
					num++;
					num2++;
				}
				i++;
			}
			complete = (i == offsetIn + lengthIn);
			usedIn = i - offsetIn;
			usedOut = num2;
		}

		public override void Reset()
		{
			this.isKana = false;
			this.isEscapeKana = false;
			this.runBeganWithEscape = false;
		}

		public override char Abbreviation
		{
			get
			{
				return 'k';
			}
		}

		private bool isKana;

		private bool isEscapeKana;

		private bool runBeganWithEscape;
	}
}
