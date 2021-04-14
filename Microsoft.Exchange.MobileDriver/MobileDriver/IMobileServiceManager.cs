using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal interface IMobileServiceManager
	{
		IMobileServiceSelector Selector { get; }

		bool CapabilityPerRecipientSupported { get; }

		MobileServiceCapability GetCapabilityForRecipient(MobileRecipient recipient);
	}
}
