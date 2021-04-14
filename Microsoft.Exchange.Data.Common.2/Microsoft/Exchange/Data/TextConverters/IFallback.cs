using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal interface IFallback
	{
		byte[] GetUnsafeAsciiMap(out byte unsafeAsciiMask);

		bool HasUnsafeUnicode();

		bool TreatNonAsciiAsUnsafe(string charset);

		bool IsUnsafeUnicode(char ch, bool isFirstChar);

		bool FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int lineBufferEnd);
	}
}
