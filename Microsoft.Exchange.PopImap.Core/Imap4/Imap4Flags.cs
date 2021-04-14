using System;

namespace Microsoft.Exchange.Imap4
{
	[Flags]
	internal enum Imap4Flags
	{
		None = 0,
		Recent = 1,
		Seen = 2,
		Deleted = 4,
		Answered = 8,
		Draft = 16,
		Flagged = 32,
		MdnSent = 64,
		Wildcard = 128,
		MimeFailed = 256,
		ItemStatus = 108,
		ItemFlags = 16
	}
}
