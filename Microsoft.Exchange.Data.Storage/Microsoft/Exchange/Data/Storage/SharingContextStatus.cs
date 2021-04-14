using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum SharingContextStatus
	{
		Uninitialized,
		TentativeConfig,
		Configured,
		Expired,
		Revoked,
		TentativeDelete,
		Tombstone,
		Modified,
		TentativeRebind,
		NotFound
	}
}
