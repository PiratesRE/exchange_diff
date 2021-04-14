using System;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	internal enum DiagnosticsAggregationEvent
	{
		LogStarted,
		Information,
		LocalViewRequestSent,
		LocalViewRequestSentFailed,
		LocalViewRequestReceived,
		LocalViewRequestReceivedFailed,
		LocalViewResponseSent,
		LocalViewResponseReceived,
		AggregatedViewRequestReceived,
		AggregatedViewRequestReceivedFailed,
		AggregatedViewResponseSent,
		QueueSnapshotFileReadSucceeded,
		QueueSnapshotFileReadFailed,
		OutOfResources,
		ServiceletError
	}
}
