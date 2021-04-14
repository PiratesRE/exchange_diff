using System;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ITraceLogger
	{
		void LogTraces(ITracer tracer);
	}
}
