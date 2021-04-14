using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal interface IMobileService
	{
		IMobileServiceManager Manager { get; }

		void Send(IList<TextSendingPackage> textPackages, Message message, MobileRecipient sender);
	}
}
