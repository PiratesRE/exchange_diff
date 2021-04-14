using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IExecutionTrackingData<TOperationData> where TOperationData : class
	{
		int Count { get; }

		TimeSpan TotalTime { get; }

		void Aggregate(TOperationData dataToAggregate);

		void AppendToTraceContentBuilder(TraceContentBuilder cb);

		void AppendDetailsToTraceContentBuilder(TraceContentBuilder cb, int indentLevel);
	}
}
