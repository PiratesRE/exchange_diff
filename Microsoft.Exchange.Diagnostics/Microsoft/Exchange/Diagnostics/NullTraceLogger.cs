using System;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NullTraceLogger : ITraceLogger
	{
		private NullTraceLogger()
		{
		}

		public void LogTraces(ITracer tracer)
		{
		}

		public static readonly NullTraceLogger Instance = new NullTraceLogger();
	}
}
