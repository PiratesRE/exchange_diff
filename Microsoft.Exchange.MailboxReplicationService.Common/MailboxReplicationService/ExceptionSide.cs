using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum ExceptionSide
	{
		None = 0,
		Source = 1,
		Target = 2
	}
}
