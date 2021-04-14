using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.Publisher
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class WorkingSetPublisherLogSchema
	{
		internal enum OperationStart
		{
			OperationName
		}

		internal enum CommandExecution
		{
			Command,
			GroupMailbox,
			UserMailboxes
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
			DedupePart,
			GroupPart,
			EnsGroupPart,
			PartAddedPub,
			PartSkippedPub,
			PubMsgId,
			PubMsgInetId,
			HasWorkingSet,
			Exception
		}
	}
}
