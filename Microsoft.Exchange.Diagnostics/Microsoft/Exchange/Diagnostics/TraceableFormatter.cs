using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class TraceableFormatter<T> : TraceFormatter<T> where T : ITraceable
	{
		public override void Format(ITraceBuilder builder, T value)
		{
			if (value != null)
			{
				value.TraceTo(builder);
				return;
			}
			builder.AddArgument(string.Empty);
		}
	}
}
