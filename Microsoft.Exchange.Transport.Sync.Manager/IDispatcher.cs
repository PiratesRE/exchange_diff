using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Manager.Throttling;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDispatcher
	{
		void Shutdown();

		DispatchResult DispatchSubscription(DispatchEntry dispatchEntry, ISubscriptionInformation subscriptionInformation);

		XElement GetDiagnosticInfo(SyncDiagnosticMode mode);
	}
}
