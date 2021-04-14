using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ConflictResult
	{
		AcceptClientChange = 1,
		RejectClientChange,
		TemporarilyUnavailable,
		ObjectNotFound
	}
}
