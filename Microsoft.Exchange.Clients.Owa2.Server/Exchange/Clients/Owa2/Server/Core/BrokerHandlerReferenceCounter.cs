using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class BrokerHandlerReferenceCounter : DisposeTrackableBase
	{
		public BrokerHandlerReferenceCounter(Func<BrokerHandler> createHandler)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.handler = createHandler();
				this.handler.Subscribe();
				disposeGuard.Success();
			}
		}

		public int Count
		{
			get
			{
				return this.associatedChannels.Count;
			}
		}

		public void Add(string channelId)
		{
			this.associatedChannels.Add(channelId ?? string.Empty);
		}

		public void Remove(string channelId)
		{
			if (string.IsNullOrEmpty(channelId))
			{
				return;
			}
			this.associatedChannels.Remove(channelId);
		}

		public void KeepAlive(ExDateTime eventTime)
		{
			if (this.handler != null)
			{
				this.handler.KeepAlive(eventTime);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.handler != null)
			{
				this.handler.Dispose();
				this.handler = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BrokerHandlerReferenceCounter>(this);
		}

		private BrokerHandler handler;

		private readonly HashSet<string> associatedChannels = new HashSet<string>();
	}
}
