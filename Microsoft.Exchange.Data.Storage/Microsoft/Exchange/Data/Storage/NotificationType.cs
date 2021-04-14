using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum NotificationType
	{
		NewMail = 1,
		Created = 2,
		Deleted = 4,
		Modified = 8,
		Moved = 16,
		Copied = 32,
		SearchComplete = 64,
		Query = 128,
		ConnectionDropped = 256
	}
}
