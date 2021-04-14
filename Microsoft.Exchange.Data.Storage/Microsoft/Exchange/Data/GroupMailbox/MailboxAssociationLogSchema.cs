using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MailboxAssociationLogSchema
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

		internal enum PerformanceCounter
		{
			CounterName,
			Context,
			Latency
		}

		internal enum PerformanceCounterName
		{
			JoinGroupAssociationReplication
		}

		internal enum Warning
		{
			Message,
			Context
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
			NewSessionRequired,
			NewSessionWrong,
			NewSessionLatency,
			AssociationsRead,
			AssociationsCreated,
			AssociationsUpdated,
			AssociationsDeleted,
			FailedAssociationsSearch,
			MissingLegacyDns,
			NonUniqueAssociationsFound,
			AutoSubscribedMembers,
			AssociationReplicationAttempts,
			FailedAssociationReplications,
			AADQueryLatency
		}
	}
}
