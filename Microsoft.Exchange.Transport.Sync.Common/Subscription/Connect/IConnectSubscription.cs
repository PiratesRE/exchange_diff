using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConnectSubscription : ISyncWorkerData
	{
		string AccessTokenInClearText { get; }

		string AccessTokenSecretInClearText { get; }

		string UserId { get; }
	}
}
