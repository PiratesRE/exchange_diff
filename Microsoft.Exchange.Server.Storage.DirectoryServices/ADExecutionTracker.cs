using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public class ADExecutionTracker
	{
		public static void Initialize()
		{
			ADExecutionTracker.adOperationTimeoutDefinition = new ThreadManager.TimeoutDefinition(ConfigurationSchema.ADOperationTimeout.Value, new Action<ThreadManager.ThreadInfo>(ADExecutionTracker.CrashOnTimeout));
		}

		public static ADExecutionTracker.DirectoryExecutionTrackingFrame TrackCall(IExecutionContext context, string operationName)
		{
			return new ADExecutionTracker.DirectoryExecutionTrackingFrame(context, operationName);
		}

		private static void CrashOnTimeout(ThreadManager.ThreadInfo threadInfo)
		{
			throw new InvalidOperationException(string.Format("Possible hang detected. Operation: {0}. Client: {2}. MailboxGuid: {3}", threadInfo.MethodName, threadInfo.Client, threadInfo.MailboxGuid));
		}

		private static ThreadManager.TimeoutDefinition adOperationTimeoutDefinition;

		public struct DirectoryExecutionTrackingFrame : IDisposable
		{
			internal DirectoryExecutionTrackingFrame(IExecutionContext context, string operationName)
			{
				ADExecutionTracker.OperationExecutionTrackableWrapper<string> operation = new ADExecutionTracker.OperationExecutionTrackableWrapper<string>(operationName);
				this.operationData = context.RecordOperation<ExecutionDiagnostics.DirectoryTrackingData>(operation);
				if (this.operationData != null)
				{
					this.operationData.Count++;
				}
				this.startTimeStamp = StopwatchStamp.GetStamp();
				this.threadManagerMethodFrame = ThreadManager.NewMethodFrame(operationName, ADExecutionTracker.adOperationTimeoutDefinition);
			}

			public void Dispose()
			{
				if (this.operationData != null)
				{
					this.operationData.ExecutionTime += this.startTimeStamp.ElapsedTime;
				}
				this.threadManagerMethodFrame.Dispose();
			}

			private ExecutionDiagnostics.DirectoryTrackingData operationData;

			private StopwatchStamp startTimeStamp;

			private ThreadManager.MethodFrame threadManagerMethodFrame;
		}

		internal class OperationExecutionTrackableWrapper<TOperation> : IOperationExecutionTrackable, IOperationExecutionTrackingKey
		{
			internal OperationExecutionTrackableWrapper(TOperation operation)
			{
				this.operation = operation;
			}

			IOperationExecutionTrackingKey IOperationExecutionTrackable.GetTrackingKey()
			{
				return this;
			}

			int IOperationExecutionTrackingKey.GetTrackingKeyHashValue()
			{
				return this.operation.GetHashCode();
			}

			public int GetSimpleHashValue()
			{
				return this.operation.GetHashCode();
			}

			bool IOperationExecutionTrackingKey.IsTrackingKeyEqualTo(IOperationExecutionTrackingKey other)
			{
				return other != null && other is ADExecutionTracker.OperationExecutionTrackableWrapper<TOperation> && this.operation.Equals(((ADExecutionTracker.OperationExecutionTrackableWrapper<TOperation>)other).operation);
			}

			string IOperationExecutionTrackingKey.TrackingKeyToString()
			{
				return this.operation.ToString();
			}

			private TOperation operation;
		}
	}
}
