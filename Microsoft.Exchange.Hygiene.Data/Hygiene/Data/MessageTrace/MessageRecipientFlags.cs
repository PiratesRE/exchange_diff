using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	[Flags]
	internal enum MessageRecipientFlags
	{
		None = 0,
		Quarantined = 1,
		Notified = 2,
		Reported = 4,
		Released = 8
	}
}
