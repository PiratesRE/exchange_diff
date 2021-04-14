using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class PooledEvent : DisposeTrackableBase
	{
		public EventWaitHandle WaitHandle
		{
			get
			{
				return this.sessionEvent;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PooledEvent>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.sessionEvent.Dispose();
				this.sessionEvent = null;
			}
		}

		private ManualResetEvent sessionEvent = new ManualResetEvent(false);
	}
}
