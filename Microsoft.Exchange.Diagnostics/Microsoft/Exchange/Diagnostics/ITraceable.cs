using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface ITraceable
	{
		void TraceTo(ITraceBuilder traceBuilder);
	}
}
