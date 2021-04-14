using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class NullExecutionContext : IExecutionContext
	{
		public static IExecutionContext Instance
		{
			get
			{
				return NullExecutionContext.context;
			}
		}

		public TOperationData RecordOperation<TOperationData>(IOperationExecutionTrackable operation) where TOperationData : class, IExecutionTrackingData<TOperationData>, new()
		{
			return default(TOperationData);
		}

		public IExecutionDiagnostics Diagnostics
		{
			get
			{
				return NullExecutionContext.diagnostics;
			}
		}

		public bool IsMailboxOperationStarted
		{
			get
			{
				return false;
			}
		}

		public bool SkipDatabaseLogsFlush
		{
			get
			{
				return false;
			}
		}

		public void OnDatabaseFailure(bool isCriticalFailure, LID lid)
		{
		}

		private static IExecutionContext context = new NullExecutionContext();

		private static IExecutionDiagnostics diagnostics = new NullExecutionDiagnostics();
	}
}
