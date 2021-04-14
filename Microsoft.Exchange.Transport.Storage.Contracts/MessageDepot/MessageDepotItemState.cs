using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal enum MessageDepotItemState
	{
		Ready,
		Deferred,
		Poisoned,
		Suspended,
		Processing,
		Expiring
	}
}
