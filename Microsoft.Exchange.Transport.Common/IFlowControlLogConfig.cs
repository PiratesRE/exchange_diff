using System;

namespace Microsoft.Exchange.Transport
{
	public interface IFlowControlLogConfig
	{
		TimeSpan AsyncInterval { get; }

		int BufferSize { get; }

		TimeSpan FlushInterval { get; }

		TimeSpan SummaryLoggingInterval { get; }

		TimeSpan SummaryBucketLength { get; }

		int MaxSummaryLinesLogged { get; }
	}
}
