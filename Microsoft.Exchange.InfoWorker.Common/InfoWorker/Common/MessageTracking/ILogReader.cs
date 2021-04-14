using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal interface ILogReader
	{
		string Server { get; }

		MtrSchemaVersion MtrSchemaVersion { get; }

		List<MessageTrackingLogEntry> ReadLogs(RpcReason rpcReason, string logFile, string messageId, DateTime startTime, DateTime endTime, TrackingEventBudget eventBudget);

		List<MessageTrackingLogEntry> ReadLogs(RpcReason rpcReason, string logFilePrefix, ProxyAddressCollection senderProxyAddresses, DateTime startTime, DateTime endTime, TrackingEventBudget eventBudget);
	}
}
