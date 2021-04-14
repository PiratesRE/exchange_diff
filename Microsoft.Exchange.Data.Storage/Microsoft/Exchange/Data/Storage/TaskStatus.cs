using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum TaskStatus
	{
		NotStarted,
		InProgress,
		Completed,
		WaitingOnOthers,
		Deferred
	}
}
