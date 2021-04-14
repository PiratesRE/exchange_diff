using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SubscriptionEventArgs<R> where R : SubscriptionEventResult
	{
		public SubscriptionEventArgs(SyncLogSession syncLogSession, R result)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentNull("result", result);
			this.result = result;
			this.syncLogSession = syncLogSession;
		}

		public R Result
		{
			get
			{
				return this.result;
			}
		}

		public SyncLogSession SyncLogSession
		{
			get
			{
				return this.syncLogSession;
			}
		}

		private readonly R result;

		private readonly SyncLogSession syncLogSession;
	}
}
