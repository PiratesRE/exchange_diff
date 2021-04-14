using System;

namespace Microsoft.Exchange.Connections.Imap
{
	[Flags]
	public enum ImapMailFlags
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
