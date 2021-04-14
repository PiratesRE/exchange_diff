using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal interface ITextSinkEx : ITextSink
	{
		void Write(string value);

		void WriteNewLine();
	}
}
