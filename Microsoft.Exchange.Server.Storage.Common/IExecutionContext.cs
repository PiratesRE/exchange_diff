using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IExecutionContext
	{
		TOperationData RecordOperation<TOperationData>(IOperationExecutionTrackable operation) where TOperationData : class, IExecutionTrackingData<TOperationData>, new();

		IExecutionDiagnostics Diagnostics { get; }

		bool IsMailboxOperationStarted { get; }

		bool SkipDatabaseLogsFlush { get; }

		void OnDatabaseFailure(bool isCriticalFailure, LID lid);
	}
}
