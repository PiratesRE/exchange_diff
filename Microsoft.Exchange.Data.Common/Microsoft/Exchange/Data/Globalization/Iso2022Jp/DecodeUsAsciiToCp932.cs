using System;

namespace Microsoft.Exchange.Data.Globalization.Iso2022Jp
{
	internal class DecodeUsAsciiToCp932 : DecodeToCp932
	{
		public override bool IsEscapeSequenceHandled(Escape escape)
		{
			return escape.Sequence == EscapeSequence.Iso646Irv;
		}

		public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut)
		{
			usedIn = 0;
			usedOut = 0;
			int i = offsetIn;
			int num = 0;
			bool flag = false;
			bool isValidEscapeSequence = escape.IsValidEscapeSequence;
			int num2 = 0;
			int limit = this.CalculateLoopCountLimit(lengthIn);
			if (isValidEscapeSequence)
			{
				if (!this.IsEscapeSequenceHandled(escape))
				{
					throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
				}
				i += escape.BytesInCurrentBuffer;
			}
			while (i < offsetIn + lengthIn)
			{
				this.CheckLoopCount(ref num2, limit);
				byte b = dataIn[i];
				if (b == 27 || b == 15 || b == 14 || (b > 127 && !isValidEscapeSequence) || b == 0)
				{
					break;
				}
				if ((b < 32 || b > 127) && b != 9 && b != 10 && b != 11 && b != 12 && b != 13)
				{
					flag = true;
				}
				i++;
				num++;
			}
			usedIn = i - offsetIn;
			usedOut = num;
			if (!flag || isValidEscapeSequence)
			{
				return ValidationResult.Valid;
			}
			return ValidationResult.Invalid;
		}

		public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete)
		{
			usedIn = 0;
			usedOut = 0;
			int i = offsetIn;
			int num = offsetOut;
			int num2 = 0;
			int limit = this.CalculateLoopCountLimit(lengthIn);
			if (escape.IsValidEscapeSequence)
			{
				if (!this.IsEscapeSequenceHandled(escape))
				{
					throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
				}
				i += escape.BytesInCurrentBuffer;
			}
			while (i < offsetIn + lengthIn)
			{
				this.CheckLoopCount(ref num2, limit);
				byte b = dataIn[i];
				if (b == 27 || b == 15 || b == 14 || b == 0)
				{
					break;
				}
				if (num + 1 > offsetOut + lengthOut)
				{
					string message = string.Format("DecodeUsAsciiToCp932.ConvertToCp932: output buffer overrun, offset {0}, length {1}", offsetOut, lengthOut);
					throw new InvalidOperationException(message);
				}
				dataOut[num++] = dataIn[i++];
			}
			complete = (i == offsetIn + lengthIn);
			usedIn = i - offsetIn;
			usedOut = num - offsetOut;
		}

		public override char Abbreviation
		{
			get
			{
				return 'a';
			}
		}
	}
}
