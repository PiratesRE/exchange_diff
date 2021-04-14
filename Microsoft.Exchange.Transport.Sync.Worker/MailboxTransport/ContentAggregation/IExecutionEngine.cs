using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IExecutionEngine
	{
		IAsyncResult BeginExecution(AggregationWorkItem workItem, AsyncCallback callback, object callbackState);

		AsyncOperationResult<SyncEngineResultData> EndExecution(IAsyncResult asyncResult);

		void Cancel(IAsyncResult asyncResult);
	}
}
