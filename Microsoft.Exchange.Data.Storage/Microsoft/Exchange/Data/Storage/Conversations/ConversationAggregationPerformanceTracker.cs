using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConversationAggregationPerformanceTracker : PerformanceTrackerBase, IMailboxPerformanceTracker, IPerformanceTracker
	{
		public ConversationAggregationPerformanceTracker(IMailboxSession mailboxSession) : base(mailboxSession)
		{
		}

		public ILogEvent GetLogEvent(string operationName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("operationName", operationName);
			base.EnforceInternalState(PerformanceTrackerBase.InternalState.Stopped, "GetLogEvent");
			return new SchemaBasedLogEvent<ConversationAggregationLogSchema.OperationEnd>
			{
				{
					ConversationAggregationLogSchema.OperationEnd.OperationName,
					operationName
				},
				{
					ConversationAggregationLogSchema.OperationEnd.Elapsed,
					base.ElapsedTime.TotalMilliseconds
				},
				{
					ConversationAggregationLogSchema.OperationEnd.CPU,
					base.CpuTime.TotalMilliseconds
				},
				{
					ConversationAggregationLogSchema.OperationEnd.RPCCount,
					base.StoreRpcCount
				},
				{
					ConversationAggregationLogSchema.OperationEnd.RPCLatency,
					base.StoreRpcLatency.TotalMilliseconds
				},
				{
					ConversationAggregationLogSchema.OperationEnd.DirectoryCount,
					base.DirectoryCount
				},
				{
					ConversationAggregationLogSchema.OperationEnd.DirectoryLatency,
					base.DirectoryLatency.TotalMilliseconds
				},
				{
					ConversationAggregationLogSchema.OperationEnd.StoreTimeInServer,
					base.StoreTimeInServer.TotalMilliseconds
				},
				{
					ConversationAggregationLogSchema.OperationEnd.StoreTimeInCPU,
					base.StoreTimeInCPU.TotalMilliseconds
				},
				{
					ConversationAggregationLogSchema.OperationEnd.StorePagesRead,
					base.StorePagesRead
				},
				{
					ConversationAggregationLogSchema.OperationEnd.StorePagesPreRead,
					base.StorePagesPreread
				},
				{
					ConversationAggregationLogSchema.OperationEnd.StoreLogRecords,
					base.StoreLogRecords
				},
				{
					ConversationAggregationLogSchema.OperationEnd.StoreLogBytes,
					base.StoreLogBytes
				}
			};
		}
	}
}
