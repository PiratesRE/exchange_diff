using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Worker.Health;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IRemoteServerHealthChecker
	{
		RemoteServerHealthState GetRemoteServerHealthState(ISyncWorkerData subscription);

		bool IsRemoteServerSlow(SyncEngineState syncEngineState, ISyncWorkerData subscription, out RemoteServerTooSlowException exception);
	}
}
