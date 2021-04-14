using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class Int64TraceFormatter : TraceFormatter<long>
	{
		public override void Format(ITraceBuilder builder, long value)
		{
			builder.AddArgument(value);
		}
	}
}
