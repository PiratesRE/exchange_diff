using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal interface IMobileActionProvider
	{
		IList<IMobileServiceManager> ServiceManagers { get; }

		void Send(Message message, MobileRecipient sender, ICollection<MobileRecipient> recipients, int maxSegmentsPerRecipient);
	}
}
