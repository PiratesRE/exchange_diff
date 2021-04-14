using System;

namespace Microsoft.Exchange.Assistants
{
	[Flags]
	internal enum MailboxType
	{
		User = 1,
		System = 2,
		Archive = 4,
		Arbitration = 8,
		PublicFolder = 16,
		InactiveMailbox = 32,
		All = 31
	}
}
