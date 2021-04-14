using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal interface IMobileServiceSelector
	{
		MobileServiceType Type { get; }

		int PersonToPersonMessagingPriority { get; }

		int MachineToPersonMessagingPriority { get; }
	}
}
