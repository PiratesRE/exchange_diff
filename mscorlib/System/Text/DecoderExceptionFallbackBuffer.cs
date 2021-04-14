using System;
using System.Globalization;

namespace System.Text
{
	public sealed class DecoderExceptionFallbackBuffer : DecoderFallbackBuffer
	{
		public override bool Fallback(byte[] bytesUnknown, int index)
		{
			this.Throw(bytesUnknown, index);
			return true;
		}

		public override char GetNextChar()
		{
			return '\0';
		}

		public override bool MovePrevious()
		{
			return false;
		}

		public override int Remaining
		{
			get
			{
				return 0;
			}
		}

		private void Throw(byte[] bytesUnknown, int index)
		{
			StringBuilder stringBuilder = new StringBuilder(bytesUnknown.Length * 3);
			int num = 0;
			while (num < bytesUnknown.Length && num < 20)
			{
				stringBuilder.Append("[");
				stringBuilder.Append(bytesUnknown[num].ToString("X2", CultureInfo.InvariantCulture));
				stringBuilder.Append("]");
				num++;
			}
			if (num == 20)
			{
				stringBuilder.Append(" ...");
			}
			throw new DecoderFallbackException(Environment.GetResourceString("Argument_InvalidCodePageBytesIndex", new object[]
			{
				stringBuilder,
				index
			}), bytesUnknown, index);
		}
	}
}
