using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class Int32TraceFormatter : TraceFormatter<int>
	{
		public override void Format(ITraceBuilder builder, int value)
		{
			builder.AddArgument(value);
		}
	}
}
