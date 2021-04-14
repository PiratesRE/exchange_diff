using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum MapiNotificationFlags : uint
	{
		TotalMessageCountChanged = 4096U,
		UnreadMessageCountChanged = 8192U,
		SearchFolder = 16384U,
		Message = 32768U
	}
}
