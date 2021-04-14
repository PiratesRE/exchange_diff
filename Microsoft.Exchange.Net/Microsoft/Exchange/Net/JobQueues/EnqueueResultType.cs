using System;

namespace Microsoft.Exchange.Net.JobQueues
{
	public enum EnqueueResultType
	{
		Successful,
		AlreadyPending,
		QueueLengthLimitReached,
		QueueServerNotInitialized,
		QueueServerShutDown,
		InvalidData,
		DirectoryError,
		StorageError,
		RpcError,
		ClientInitError,
		UnexpectedServerError,
		RequestThrottled,
		UnlinkedTeamMailbox,
		WrongServer,
		UnknownError,
		PendingDeleteTeamMailbox,
		ClosedTeamMailbox,
		NonexistentTeamMailbox
	}
}
