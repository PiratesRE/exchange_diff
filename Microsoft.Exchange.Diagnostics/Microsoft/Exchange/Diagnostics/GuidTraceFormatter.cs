using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class GuidTraceFormatter : TraceFormatter<Guid>
	{
		public override void Format(ITraceBuilder builder, Guid value)
		{
			builder.AddArgument(value);
		}
	}
}
