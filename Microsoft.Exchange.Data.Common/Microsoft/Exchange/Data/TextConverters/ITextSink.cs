using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal interface ITextSink
	{
		bool IsEnough { get; }

		void Write(char[] buffer, int offset, int count);

		void Write(int ucs32Char);
	}
}
