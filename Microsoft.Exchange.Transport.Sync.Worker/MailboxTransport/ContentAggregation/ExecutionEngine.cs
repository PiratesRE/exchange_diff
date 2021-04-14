using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ExecutionEngine : IExecutionEngine
	{
		public abstract IAsyncResult BeginExecution(AggregationWorkItem workItem, AsyncCallback callback, object callbackState);

		public abstract AsyncOperationResult<SyncEngineResultData> EndExecution(IAsyncResult asyncResult);

		public virtual void Cancel(IAsyncResult asyncResult)
		{
		}

		internal static readonly Trace Tracer = ExTraceGlobals.SyncEngineTracer;
	}
}
