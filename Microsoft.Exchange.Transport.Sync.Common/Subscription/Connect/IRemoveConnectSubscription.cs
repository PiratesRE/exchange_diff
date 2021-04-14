using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IRemoveConnectSubscription
	{
		void TryRemovePermissions(IConnectSubscription subscription);
	}
}
