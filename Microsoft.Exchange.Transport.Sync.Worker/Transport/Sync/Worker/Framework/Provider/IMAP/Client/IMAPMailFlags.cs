using System;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[Flags]
	internal enum IMAPMailFlags
	{
		None = 0,
		Answered = 1,
		Flagged = 2,
		Deleted = 4,
		Seen = 8,
		Draft = 16,
		All = 31
	}
}
