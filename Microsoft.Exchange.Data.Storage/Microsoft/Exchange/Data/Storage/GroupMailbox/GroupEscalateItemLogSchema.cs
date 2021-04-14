using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class GroupEscalateItemLogSchema
	{
		internal enum OperationStart
		{
			OperationName
		}

		internal enum Error
		{
			Exception,
			Context
		}

		internal enum OperationEnd
		{
			OperationName,
			Elapsed,
			CPU,
			RPCCount,
			RPCLatency,
			DirectoryCount,
			DirectoryLatency,
			StoreTimeInServer,
			StoreTimeInCPU,
			StorePagesRead,
			StorePagesPreRead,
			StoreLogRecords,
			StoreLogBytes,
			OrigMsgSender,
			OrigMsgSndRcpType,
			OrigMsgClass,
			OrigMsgId,
			OrigMsgInetId,
			PartOrigMsg,
			GroupReplyTo,
			GroupPart,
			EnsGroupPart,
			DedupePart,
			PartAddedEsc,
			PartSkippedEsc,
			HasEscalated,
			GroupReplyToSkipped,
			SendToYammer,
			SendToYammerMs,
			UnsubscribeUrl,
			UnsubscribeUrlBuildMs,
			UnsubscribeBodySize,
			UnsubscribeUrlDetectionMs,
			UnsubscribeUrlInsertMs
		}
	}
}
