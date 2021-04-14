using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ContentSyncPhase
	{
		None,
		Change,
		Delete,
		ReadUnread
	}
}
