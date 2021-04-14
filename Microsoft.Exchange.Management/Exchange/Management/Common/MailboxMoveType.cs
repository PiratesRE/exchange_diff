using System;

namespace Microsoft.Exchange.Management.Common
{
	[Flags]
	public enum MailboxMoveType
	{
		IsPrimaryMoving = 1,
		IsArchiveMoving = 2
	}
}
